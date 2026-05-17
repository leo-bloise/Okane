using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Okane.Authentication;
using Okane.Authentication.Repositories;
using Okane.Authentication.Requests;
using Okane.Authentication.Services;
using Okane.Infrastructure.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Okane.Infrastructure.Services;

public class AuthenticationService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IOptions<JwtOptions> jwtOptions
): IAuthenticationService {
    private string CreateToken(User user)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.UTF8Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey));

        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        Claim[] claims = [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.ToString()),
        ];

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: jwtOptions.Value.Issuer,
            audience: jwtOptions.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> LoginAsync(ILoginUserRequest request, CancellationToken cancellationToken)
    {
        User? user = await userRepository.FindByEmailAsync(request.Email, cancellationToken);

        if (user is null) throw new Exception("Not authorized");

        if (!await passwordHasher.VerifyPasswordAsync(request.Password, user.Password, cancellationToken))
            throw new Exception("Not authorized");

        return CreateToken(user);
    }

    public async Task<User> RegisterUserAsync(IRegisterUserRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        if(await userRepository.ExistsByEmailAsync(request.Email, cancellationToken)) // change for domain exception
            throw new Exception("Email already taken");

        User user = new User(request.Email, await passwordHasher.HashPasswordAsync(request.Password, cancellationToken));

        await userRepository.SaveUserAsync(user, cancellationToken);

        return user;
    }

}
