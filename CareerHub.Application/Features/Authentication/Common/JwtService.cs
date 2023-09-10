using CareerHub.Domain.Entities.User;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CareerHub.Application.Features.Authentication.Common
{
    public static class JwtService
    {
        public static string GenerateJwtToken(UserModel user, IConfiguration configuration)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"] ?? "Default_Super_Secret_256_Bits_Signing_Key"));

            var tokenLifetime = configuration.GetValue<double>("JwtSettings:AccessTokenExpirationInMinutes", 60);

            var securityToken = new JwtSecurityToken(
                issuer: configuration["JwtSettings:ValidIssuer"],
                audience: configuration["JwtSettings:ValidAudience"],
                expires: DateTime.UtcNow.AddMinutes(tokenLifetime),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }
    }
}
