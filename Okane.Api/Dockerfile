# Build context must be the repository root (contains all project references), e.g.:
#   docker build -f Okane.Api/Dockerfile -t okane-api .

# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files first so restore is cached independently of source changes.
COPY Okane.Api/Okane.Api.csproj Okane.Api/
COPY Okane.Kernel/Okane.Kernel.csproj Okane.Kernel/
COPY Okane.User/Okane.User.csproj Okane.User/
COPY Okane.Wallet/Okane.Wallet.csproj Okane.Wallet/
COPY Okane.Transaction/Okane.Transaction.csproj Okane.Transaction/
RUN dotnet restore Okane.Api/Okane.Api.csproj

COPY Okane.Api/ Okane.Api/
COPY Okane.Kernel/ Okane.Kernel/
COPY Okane.User/ Okane.User/
COPY Okane.Wallet/ Okane.Wallet/
COPY Okane.Transaction/ Okane.Transaction/

# Release configuration excludes the dev-only Okane.Seeder project reference (see Okane.Api.csproj).
RUN dotnet publish Okane.Api/Okane.Api.csproj -c Release --no-restore -o /app/publish

# appsettings.json/appsettings.Development.json are not part of the runtime image;
# all configuration is supplied via environment variables instead.
RUN rm -f /app/publish/appsettings.json /app/publish/appsettings.Development.json

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=8080

# --- Logging ---
ENV Logging__LogLevel__Default=Information
ENV Logging__LogLevel__Microsoft.AspNetCore=Warning

# --- Observability (Infrastructure/Observability/ObservabilityExtensions.cs) ---
ENV Observability__ServiceName=Okane.Api
ENV Observability__OtlpEndpoint=http://localhost:4317

# --- ASP.NET Core host ---
ENV AllowedHosts=*

# --- Database connection (Program.cs) ---
ENV ConnectionStrings__Okane=

# --- JWT authentication (Infrastructure/Security/JwtOptions.cs) ---
ENV Jwt__Issuer=Okane.Api
ENV Jwt__Audience=Okane.Api
ENV Jwt__SigningKey=
ENV Jwt__ExpiryMinutes=60

# --- CORS (Infrastructure/Security/CorsOptions.cs) ---
ENV Cors__Origins__0=

EXPOSE 8080

ENTRYPOINT ["dotnet", "Okane.Api.dll"]
