output "instance_public_ip" {
  description = "Stable public (Elastic) IP of the Okane EC2 instance. Survives instance replacement."
  value       = aws_eip.okane.public_ip
}

output "instance_public_dns" {
  description = "Public DNS name of the Okane EC2 instance."
  value       = aws_instance.okane.public_dns
}

output "ssh_command" {
  description = "Command to SSH into the instance."
  value       = "ssh ubuntu@${aws_eip.okane.public_ip}"
}

output "app_url" {
  description = "URL the client app is reachable at over plain HTTP once docker compose finishes starting."
  value       = "http://${aws_eip.okane.public_ip}"
}

output "grafana_url" {
  description = "Grafana dashboard, reachable on the same domain as the app but on port 3000 (plain HTTP - no TLS on this port)."
  value       = "http://${local.env_vars["DOMAIN"]}:3000"
}

output "dns_setup_instructions" {
  description = "DNS record to create at your DNS provider (Cloudflare) before running init-https.sh."
  value       = "Create a DNS-only (not proxied) A record: ${local.env_vars["DOMAIN"]} -> ${aws_eip.okane.public_ip}"
}

output "https_setup_command" {
  description = "One-time command to run over SSH once DNS has propagated, to obtain the certificate and switch nginx to HTTPS."
  value       = "ssh ubuntu@${aws_eip.okane.public_ip} 'sudo /opt/okane/init-https.sh'"
}

output "db_endpoint" {
  description = "RDS Postgres endpoint (host:port)."
  value       = aws_db_instance.okane.endpoint
}

output "db_port" {
  description = "RDS Postgres port."
  value       = aws_db_instance.okane.port
}

output "db_secret_arn" {
  description = "Secrets Manager ARN holding the RDS master password. Fetch with: aws secretsmanager get-secret-value --secret-id <arn> --query SecretString --output text"
  value       = aws_db_instance.okane.master_user_secret[0].secret_arn
}

output "db_tunnel_command" {
  description = "Command to open an SSH tunnel to RDS for ad-hoc psql access (RDS has no public IP by design)."
  value       = "ssh -L 5432:${aws_db_instance.okane.address}:${aws_db_instance.okane.port} ubuntu@${aws_eip.okane.public_ip}"
}
