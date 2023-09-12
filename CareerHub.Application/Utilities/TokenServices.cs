using CareerHub.Application.Configurations;
using CareerHub.Domain.Entities.User;
using CareerHub.Domain.ViewModels;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CareerHub.Application.Utilities
{
    public class TokenServices
    {
        private readonly JwtSettings _jwtSettings;

        // Constants for claim types
        private const string ClaimTypeUserId = ClaimTypes.NameIdentifier;
        private const string ClaimTypeEmail = ClaimTypes.Email;
        private const string ClaimTypeEmailVerified = ClaimTypes.AuthorizationDecision;
        private const string ClaimTypeLastLogin = ClaimTypes.Expiration;

        public TokenServices(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
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

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            var securityToken = new JwtSecurityToken(
                issuer: _jwtSettings.ValidIssuer,
                audience: _jwtSettings.ValidAudience,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes),
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
