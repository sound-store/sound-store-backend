using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SoundStore.Core.Commons;
using SoundStore.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SoundStore.Service.Test
{
    public class TokenServiceUnitTest
    {
        private readonly Mock<IOptions<JwtSettings>> _mockOptions = new();
        private readonly JwtSettings _jwtSettings;

        public TokenServiceUnitTest()
        {
            // Create test JWT settings
            _jwtSettings = new JwtSettings
            {
                Issuer = "test-issuer",
                Audience = "test-audience",
                Key = "this-is-a-test-key-that-needs-to-be-at-least-16-characters-long-for-hmacsha256"
            };

            // Setup the mock Options
            _mockOptions.Setup(x => x.Value).Returns(_jwtSettings);
        }

        [Fact]
        public void GenerateToken_ShouldReturnValidToken_WhenValidInputsProvided()
        {
            // Arrange
            var tokenService = new TokenService(_mockOptions.Object);
            var user = new AppUser
            {
                Id = "user123",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };
            string role = "Customer";

            // Act
            var token = tokenService.GenerateToken(user, role);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            // Validate token structure and claims
            var tokenHandler = new JwtSecurityTokenHandler();
            Assert.True(tokenHandler.CanReadToken(token));

            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Verify standard claims
            Assert.Equal(_jwtSettings.Issuer, jwtToken.Issuer);
            Assert.Contains(_jwtSettings.Audience, jwtToken.Audiences);
            Assert.True(jwtToken.ValidTo > DateTime.UtcNow);

            // Verify custom claims
            // Verify custom claims safely
            Assert.Equal(user.Id, jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sid)?.Value);
            Assert.Equal(user.FirstName, jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value);
            Assert.Equal(user.LastName, jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.GivenName)?.Value);
            Assert.Equal(user.Email, jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value);
        }

        [Fact]
        public void GenerateToken_ShouldHandleNullFields_WhenUserHasNullProperties()
        {
            // Arrange
            var tokenService = new TokenService(_mockOptions.Object);
            var user = new AppUser
            {
                Id = "user456",
                FirstName = null,
                LastName = null,
                Email = null
            };
            string role = "Admin";

            // Act
            var token = tokenService.GenerateToken(user, role);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Verify null fields are replaced with empty strings
            Assert.Equal(string.Empty, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value);
            Assert.Equal(string.Empty, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.GivenName).Value);
            Assert.Equal(string.Empty, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        }

        [Fact]
        public void GenerateToken_ShouldCreateValidSignature_UsingSpecifiedAlgorithm()
        {
            // Arrange
            var tokenService = new TokenService(_mockOptions.Object);
            var user = new AppUser { Id = "user789" };
            string role = "Customer";

            // Act
            var token = tokenService.GenerateToken(user, role);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Verify we can validate the token with our key
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(_jwtSettings.Key)),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);

            // Verify signature algorithm
            var securityKey = securityToken as JwtSecurityToken;
            Assert.NotNull(securityKey);
            
            Assert.Equal(SecurityAlgorithms.HmacSha256, 
                securityKey.SignatureAlgorithm, 
                StringComparer.OrdinalIgnoreCase);
        }
    }
}
