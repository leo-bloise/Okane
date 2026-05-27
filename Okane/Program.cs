using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Okane.Accounts.Repositories;
using Okane.Accounts.Services;
using Okane.Authentication.Repositories;
using Okane.Authentication.Services;
using Okane.Infrastructure;
using Okane.Infrastructure.ExceptionHandler;
using Okane.Infrastructure.Options;
using Okane.Infrastructure.Repositories;
using Okane.Infrastructure.Services;

namespace Okane;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<ValidationErrorResponseFactory>();

        builder.Services.Configure<JwtOptions>(builder.Configuration.GetRequiredSection("JwtOptions"));

        builder.Services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                var jwtOptions = builder.Configuration.GetRequiredSection("JwtOptions").Get<JwtOptions>();

                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services
            .AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var validationResponseFactory = context.HttpContext.RequestServices.GetRequiredService<ValidationErrorResponseFactory>();
                    return validationResponseFactory.Build(context.ModelState);
                };
            });

        builder.Logging.AddConsole();

        builder.Services
            .AddDbContext<OkaneDbContext>(
                options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

        builder.Services
            .AddScoped<IUserRepository, UserRepository>();
        builder.Services
            .AddScoped<IAccountRepository, AccountRepository>();

        builder.Services
            .AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        builder.Services
            .AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services
            .AddScoped<IAccountService, AccountService>();

        builder
            .Services
            .AddExceptionHandler<DomainExceptionHandler>();
        builder
            .Services
            .AddExceptionHandler<DetailedExceptionHandler>();

        builder.Services.AddProblemDetails();

        WebApplication app = builder.Build();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseExceptionHandler();

        app.MapControllers();

        app.Run();
    }
}
