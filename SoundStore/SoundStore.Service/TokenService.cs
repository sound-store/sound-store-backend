using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SoundStore.Core.Commons;
using SoundStore.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SoundStore.Service
{
    public sealed class TokenService(IOptions<JwtSettings> options)
    {
        private readonly JwtSettings _options = options.Value;

        public string GenerateToken(AppUser user)
        {
            var key = Encoding.ASCII.GetBytes(_options.Key);
            var securityKey = new SymmetricSecurityKey(key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FirstName ?? string.Empty),
                    new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    //new Claim(ClaimTypes.Role, role.ToString())
                ]),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
