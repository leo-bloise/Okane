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

output "dns_setup_instructions" {
  description = "DNS record to create at your DNS provider (Cloudflare) before running init-https.sh."
  value       = "Create a DNS-only (not proxied) A record: ${local.env_vars["DOMAIN"]} -> ${aws_eip.okane.public_ip}"
}

output "https_setup_command" {
  description = "One-time command to run over SSH once DNS has propagated, to obtain the certificate and switch nginx to HTTPS."
  value       = "ssh ubuntu@${aws_eip.okane.public_ip} 'sudo /opt/okane/init-https.sh'"
}
