data "aws_ami" "ubuntu" {
  most_recent = true
  owners      = ["099720109477"] # Canonical

  filter {
    name   = "name"
    values = ["ubuntu/images/hvm-ssd/ubuntu-jammy-22.04-amd64-server-*"]
  }

  filter {
    name   = "virtualization-type"
    values = ["hvm"]
  }
}

data "aws_vpc" "default" {
  default = true
}

data "aws_subnets" "default" {
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.default.id]
  }
}

resource "aws_security_group" "okane" {
  name        = "${var.project_name}-sg"
  description = "Okane host: SSH plus HTTP/HTTPS for the app"
  vpc_id      = data.aws_vpc.default.id

  ingress {
    description = "SSH"
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = var.ssh_allowed_cidr_blocks
  }

  ingress {
    description = "HTTP"
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    description = "HTTPS"
    from_port   = 443
    to_port     = 443
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    description = "All outbound traffic"
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name = "${var.project_name}-sg"
  }
}

resource "aws_key_pair" "okane" {
  key_name   = "${var.project_name}-key"
  public_key = var.ssh_public_key
}

locals {
  # docker-compose.yml bind-mounts these by relative path (./otel/..., ./prometheus/...,
  # ./grafana/..., ./Migrations/...). Docker silently creates an empty directory for a
  # missing bind-mount source instead of failing, so every one of these files has to
  # actually exist on the instance before `docker compose up` runs.
  support_dirs = ["otel", "prometheus", "grafana", "Migrations", "nginx"]

  support_files = merge([
    for dir in local.support_dirs : {
      for f in fileset("${path.module}/../${dir}", "**") :
      "${dir}/${f}" => file("${path.module}/../${dir}/${f}")
    }
  ]...)

  # Single source of truth for app/runtime config: the same .env docker-compose
  # reads locally is shipped byte-for-byte to /opt/okane/.env on the instance
  # (see .env.example for the documented, committed template). Not managed as
  # Terraform variables so local dev and the deployed instance can't drift.
  env_file_content = file("${path.module}/../.env")

  env_lines = [
    for line in split("\n", local.env_file_content) : trimspace(line)
    if trimspace(line) != "" && !startswith(trimspace(line), "#")
  ]

  env_pairs = [for line in local.env_lines : regex("^([^=]+)=(.*)$", line)]
  env_vars  = { for pair in local.env_pairs : pair[0] => pair[1] }
}

resource "aws_instance" "okane" {
  ami                         = data.aws_ami.ubuntu.id
  instance_type               = var.instance_type
  subnet_id                   = data.aws_subnets.default.ids[0]
  vpc_security_group_ids      = [aws_security_group.okane.id]
  key_name                    = aws_key_pair.okane.key_name
  associate_public_ip_address = true

  root_block_device {
    volume_size = var.root_volume_size_gb
    volume_type = "gp3"
  }

  # docker-compose.yml pulls bloiseleo/okane.backend and bloiseleo/okane.frontend
  # from Docker Hub (see repo root), so the instance only needs Docker + the
  # compose file itself, not the full source tree.
  #
  # Gzipped because the rendered script (compose file + otel/prometheus/grafana/
  # migrations/nginx configs all inlined) exceeds EC2's 16KB raw user-data limit;
  # cloud-init auto-detects and decompresses a gzip-compressed payload.
  user_data_base64 = base64gzip(templatefile("${path.module}/templates/user_data.sh.tftpl", {
    ssh_user               = "ubuntu"
    env_file_content       = sensitive(chomp(local.env_file_content))
    docker_compose_content = file("${path.module}/../docker-compose.yml")
    support_files          = local.support_files
    init_https_script      = file("${path.module}/templates/init-https.sh")
  }))
  user_data_replace_on_change = true

  tags = {
    Name = var.project_name
  }
}

# Stable public IP that survives instance replacement (e.g. from user_data
# changes triggering user_data_replace_on_change) - required for DNS/TLS to
# keep working without a manual DNS update every time.
resource "aws_eip" "okane" {
  instance = aws_instance.okane.id
  domain   = "vpc"

  tags = {
    Name = "${var.project_name}-eip"
  }
}
