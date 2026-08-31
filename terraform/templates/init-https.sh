#!/bin/bash
set -euo pipefail
cd /opt/okane

set -a
source ./.env
set +a

if [ -z "${DOMAIN:-}" ]; then
  echo "DOMAIN is not set in /opt/okane/.env" >&2
  exit 1
fi

echo "Requesting a Let's Encrypt certificate for ${DOMAIN}..."
echo "(this will fail if DNS for ${DOMAIN} isn't pointed at this host's IP yet)"
docker compose run --rm --entrypoint certbot certbot certonly \
  --webroot -w /var/www/certbot \
  -d "${DOMAIN}" \
  --email "${LETSENCRYPT_EMAIL}" \
  --agree-tos --no-eff-email --non-interactive

echo "Certificate obtained. Switching nginx to the HTTPS config..."
cp nginx/reverse-proxy.https.conf.template nginx/active.conf.template
docker compose restart reverse-proxy

echo "Done. Verify with: curl -I https://${DOMAIN}"
