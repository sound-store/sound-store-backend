using Microsoft.AspNetCore.Http;
using Moq;

namespace SoundStore.Service.Test
{
    public class UserClaimsServiceUnitTest
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly UserClaimsService _userClaimsService;

        public UserClaimsServiceUnitTest()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _userClaimsService = new UserClaimsService(_mockHttpContextAccessor.Object);
        }

        [Fact]
        public void GetClaim_ShouldReturnClaimValue_WhenClaimExists()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var claims = new Dictionary<string, string>
                {
                    { "userId", "123" },
                    { "role", "Admin" }
                };
            httpContext.Items["UserClaims"] = claims;

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _userClaimsService.GetClaim("userId");

            // Assert
            Assert.Equal("123", result);
        }

        [Fact]
        public void GetClaim_ShouldReturnEmptyString_WhenClaimDoesNotExist()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var claims = new Dictionary<string, string>
                {
                    { "userId", "123" },
                    { "role", "Admin" }
                };
            httpContext.Items["UserClaims"] = claims;

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _userClaimsService.GetClaim("nonExistentKey");

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetClaim_ShouldReturnEmptyString_WhenHttpContextIsNull()
        {
            // Arrange
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext)null);

            // Act
            var result = _userClaimsService.GetClaim("userId");

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetClaim_ShouldReturnEmptyString_WhenUserClaimsItemIsNull()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            // Not setting Items["UserClaims"]

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _userClaimsService.GetClaim("userId");

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetClaim_ShouldReturnEmptyString_WhenUserClaimsIsNotDictionary()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserClaims"] = "not a dictionary";

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _userClaimsService.GetClaim("userId");

            // Assert
            Assert.Equal(string.Empty, result);
        }
    }
}
