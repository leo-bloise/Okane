resource "aws_db_subnet_group" "okane" {
  name       = "${var.project_name}-db-subnet-group"
  subnet_ids = data.aws_subnets.default.ids

  tags = {
    Name = "${var.project_name}-db-subnet-group"
  }
}

resource "aws_security_group" "rds" {
  name        = "${var.project_name}-rds-sg"
  description = "Okane RDS: Postgres reachable only from the app instance"
  vpc_id      = data.aws_vpc.default.id

  ingress {
    description     = "Postgres from the app instance"
    from_port       = 5432
    to_port         = 5432
    protocol        = "tcp"
    security_groups = [aws_security_group.okane.id]
  }

  tags = {
    Name = "${var.project_name}-rds-sg"
  }
}

resource "aws_db_instance" "okane" {
  identifier     = "${var.project_name}-db"
  engine         = "postgres"
  engine_version = var.db_engine_version

  instance_class    = var.db_instance_class
  allocated_storage = var.db_allocated_storage_gb
  storage_type      = "gp3"
  storage_encrypted = true

  db_name  = "okane"
  username = "okane"

  manage_master_user_password = true

  db_subnet_group_name   = aws_db_subnet_group.okane.name
  vpc_security_group_ids = [aws_security_group.rds.id]
  publicly_accessible    = false

  backup_retention_period = var.db_backup_retention_days
  deletion_protection     = var.db_deletion_protection
  skip_final_snapshot     = var.db_skip_final_snapshot

  tags = {
    Name = "${var.project_name}-db"
  }
}

# Lets the app instance fetch the RDS-managed master password from Secrets
# Manager at boot (see templates/user_data.sh.tftpl) instead of it ever being
# written to the repo or Terraform state in plaintext.
resource "aws_iam_role" "okane_ec2" {
  name = "${var.project_name}-ec2-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Effect    = "Allow"
      Principal = { Service = "ec2.amazonaws.com" }
      Action    = "sts:AssumeRole"
    }]
  })
}

resource "aws_iam_role_policy" "okane_ec2_db_secret" {
  name = "${var.project_name}-read-db-secret"
  role = aws_iam_role.okane_ec2.id

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Effect   = "Allow"
      Action   = "secretsmanager:GetSecretValue"
      Resource = aws_db_instance.okane.master_user_secret[0].secret_arn
    }]
  })
}

resource "aws_iam_instance_profile" "okane" {
  name = "${var.project_name}-ec2-profile"
  role = aws_iam_role.okane_ec2.name
}
