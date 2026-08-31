#!/bin/bash
set -euo pipefail
cd /opt/okane

# Extract only the two keys this script needs, rather than sourcing the whole
# .env - other values (e.g. DB_CONNECTION_STRING) are .NET connection strings
# containing unescaped ';', which bash would parse as command separators,
# silently truncating/mangling them if sourced. A truncated value then leaks
# into any `docker compose` call below, since Compose prefers shell env vars
# over the .env file - corrupting the deployed container's config.
DOMAIN=$(grep -m1 '^DOMAIN=' .env | cut -d= -f2-)
LETSENCRYPT_EMAIL=$(grep -m1 '^LETSENCRYPT_EMAIL=' .env | cut -d= -f2-)

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
