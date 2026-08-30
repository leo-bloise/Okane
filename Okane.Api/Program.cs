using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Okane.Api.Contracts;
using Okane.Api.Infrastructure;
using Okane.Api.Infrastructure.ErrorHandling;
using Okane.Api.Infrastructure.Observability;
using Okane.Api.Infrastructure.Persistence;
using Okane.Api.Infrastructure.Security;
using Okane.Api.Infrastructure.UseCases;
using Okane.Api.Reports;
using Okane.Kernel;
using Okane.Transaction.Application;
using Okane.Transaction.Application.Interfaces;
using Okane.User.Application;
using Okane.User.Application.Interfaces;
using Okane.Wallet.Application;
using Okane.Wallet.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddOkaneLogging();
builder.AddOkaneTracing();
builder.AddOkaneMetrics();

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

        var response = ApiResponseFactory.Error("One or more validation errors occurred.", StatusCodes.Status422UnprocessableEntity, errors);

        return new UnprocessableEntityObjectResult(response);
    };
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtOptions>(jwtSection);

var corsSection = builder.Configuration.GetSection("Cors");
builder.Services.Configure<CorsOptions>(corsSection);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["SigningKey"]!)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies[AuthCookieNames.AccessToken];
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

var corsOptions = corsSection.Get<CorsOptions>()
    ?? throw new InvalidOperationException("Cors configuration section is missing.");

builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
        {
            policy.WithOrigins(corsOptions.Origins);
            policy.AllowAnyMethod();
            policy.AllowAnyHeader();
            policy.AllowCredentials();
        });
});
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddSingleton(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("Okane")
        ?? throw new InvalidOperationException("Connection string 'Okane' is not configured.");

    return new NpgsqlConnectionFactory(connectionString);
});
builder.Services.AddScoped<IDbConnectionProvider<NpgsqlConnection>, NpgsqlConnectionProvider>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<WalletRepository>();
builder.Services.AddScoped<IWalletRepository>(sp => sp.GetRequiredService<WalletRepository>());
builder.Services.AddScoped<IWalletLookup>(sp => sp.GetRequiredService<WalletRepository>());
builder.Services.AddScoped<IWalletService, WalletService>();

builder.Services.AddScoped<TransactionRepository>();
builder.Services.AddScoped<ITransactionRepository>(sp => sp.GetRequiredService<TransactionRepository>());
builder.Services.AddScoped<IWalletActivityChecker>(sp => sp.GetRequiredService<TransactionRepository>());
builder.Services.AddScoped<IReadLedgerRepository, ReadLedgerRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();

builder.Services.AddScoped<IDashboardReportRepository, DashboardReportRepository>();
builder.Services.AddScoped<DashboardReportService>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages(async statusCodeContext =>
{
    var response = statusCodeContext.HttpContext.Response;
    if (response.HasStarted || response.ContentLength.HasValue || !string.IsNullOrEmpty(response.ContentType))
    {
        return;
    }

    var apiResponse = ApiResponseFactory.Error(
        ReasonPhrases.GetReasonPhrase(response.StatusCode),
        response.StatusCode);

    response.ContentType = "application/json";
    await response.WriteAsJsonAsync(apiResponse);
});
app.UseCors("default");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
