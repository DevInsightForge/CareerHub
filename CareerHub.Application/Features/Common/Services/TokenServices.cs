using CareerHub.Application.Features.Common.ViewModels;
using CareerHub.Domain.Entities.User;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CareerHub.Application.Features.Common.Services
{
    public class TokenServices
    {
        // Constants for claim types
        private const string ClaimTypeUserId = ClaimTypes.NameIdentifier;
        private const string ClaimTypeEmail = ClaimTypes.Email;
        private const string ClaimTypeEmailVerified = ClaimTypes.AuthorizationDecision;
        private const string ClaimTypeLastLogin = ClaimTypes.Expiration;

        // Configuration keys
        private const string JwtSecretKeyConfigKey = "JwtSettings:SecretKey";
        private const string JwtIssuerConfigKey = "JwtSettings:ValidIssuer";
        private const string JwtAudienceConfigKey = "JwtSettings:ValidAudience";
        private const string JwtAccessTokenExpirationConfigKey = "JwtSettings:AccessTokenExpirationInMinutes";

        private readonly string _jwtSecretKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly double _jwtAccessTokenExpirationInMinutes;

        public TokenServices(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            _jwtSecretKey = configuration[JwtSecretKeyConfigKey] ?? "Default_Super_Secret_256_Bits_Signing_Key";
            _jwtIssuer = configuration[JwtIssuerConfigKey] ?? "DefaultIssuer";
            _jwtAudience = configuration[JwtAudienceConfigKey] ?? "DefaultAudience";
            _jwtAccessTokenExpirationInMinutes = configuration.GetValue<double>(JwtAccessTokenExpirationConfigKey, 60);
        }

        public string GenerateJwtToken(UserModel user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypeUserId, user.Id.Value.ToString(), ClaimValueTypes.String),
                new Claim(ClaimTypeEmail, user.Email, ClaimValueTypes.Email),
                new Claim(ClaimTypeEmailVerified, user.IsEmailVerified.ToString(), ClaimValueTypes.Boolean),
                new Claim(ClaimTypeLastLogin, user.LastLogin.ToString("yyyy-MM-dd'T'HH:mm:ss.fffZ"), ClaimValueTypes.DateTime)
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));

            var securityToken = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                expires: DateTime.UtcNow.AddMinutes(_jwtAccessTokenExpirationInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }

        public static TokenUserModel TokenUserFromClaims(ClaimsPrincipal principal)
        {
            ArgumentNullException.ThrowIfNull(principal, nameof(principal));

            TokenUserModel tokenUser = new()
            {
                UserId = Guid.TryParse(principal.FindFirstValue(ClaimTypeUserId)?.Trim(), out var parsedUserId) ? parsedUserId : Guid.Empty,
                Email = principal.FindFirstValue(ClaimTypeEmail)?.Trim() ?? string.Empty,
                IsEmailVerified = bool.TryParse(principal.FindFirstValue(ClaimTypeEmailVerified)?.Trim(), out var parsedIsEmailVerified) && parsedIsEmailVerified,
                LastLogin = DateTimeOffset.TryParse(principal.FindFirstValue(ClaimTypeLastLogin)?.Trim(), out var parsedLastLogin) ? parsedLastLogin : DateTimeOffset.MinValue
            };

            return tokenUser;
        }
    }
}
