variable "aws_region" {
  description = "AWS region to deploy into."
  type        = string
  default     = "sa-east-1"
}

variable "project_name" {
  description = "Name prefix used to tag and name all resources."
  type        = string
  default     = "okane"
}

variable "instance_type" {
  description = "EC2 instance type. The instance runs Postgres, Grafana, Prometheus, the OTel collector, and the app containers together, so avoid t3.micro/nano."
  type        = string
  default     = "t3.small"
}

variable "root_volume_size_gb" {
  description = "Root EBS volume size in GB."
  type        = number
  default     = 30
}

variable "ssh_public_key" {
  description = "Contents of the SSH public key (e.g. ~/.ssh/id_ed25519.pub) used to log into the instance. No default on purpose: never commit a real key into source control."
  type        = string
}

variable "ssh_allowed_cidr_blocks" {
  description = "CIDR blocks allowed to reach the instance over SSH (port 22). Defaults to open; restrict this to your own IP in production, e.g. [\"203.0.113.4/32\"]."
  type        = list(string)
  default     = ["0.0.0.0/0"]
}

# IMAGE_TAG, DOMAIN, LETSENCRYPT_EMAIL, and JWT_SIGNING_KEY are no longer
# Terraform variables - they live in ../.env (gitignored; see ../.env.example),
# which main.tf reads directly and ships to the instance. DNS for DOMAIN is
# managed outside AWS (Cloudflare), so Terraform can't create that record
# itself - point it at the instance's Elastic IP output (dns_setup_instructions)
# as a DNS-only (unproxied) A record.
