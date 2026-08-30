# Build context must be the repository root, e.g.:
#   docker build -f Okane.Client/Dockerfile -t okane-client .

# ---- Build stage ----
FROM node:22-alpine AS build
WORKDIR /src

COPY Okane.Client/package.json Okane.Client/package-lock.json ./
RUN npm ci

COPY Okane.Client/ ./
RUN npm run build

# ---- Runtime stage ----
FROM nginx:1.27-alpine AS runtime

# Processed by the nginx image's entrypoint via envsubst on container start,
# so the listen port and API upstream are configurable through env vars.
COPY Okane.Client/nginx.conf.template /etc/nginx/templates/default.conf.template

COPY --from=build /src/dist/Okane.Client/browser /usr/share/nginx/html

# Backend base URL the /api/ location proxies to (see src/environments/environment.ts,
# which builds the app to call the relative path "/api").
ENV API_URL=http://okane-api:8080
ENV NGINX_PORT=80

EXPOSE 80
