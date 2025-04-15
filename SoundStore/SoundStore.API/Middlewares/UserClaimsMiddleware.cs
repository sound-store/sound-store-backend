using System.IdentityModel.Tokens.Jwt;

namespace SoundStore.API.Middlewares
{
    /// <summary>
    /// Middleware for handling user claims from JWT tokens in the request pipeline.
    /// Extracts and processes claims from Bearer tokens in the Authorization header.
    /// </summary>
    public class UserClaimsMiddleware(ILogger<UserClaimsMiddleware> logger) : IMiddleware
    {
        private readonly ILogger<UserClaimsMiddleware> _logger = logger;

        /// <summary>
        /// Processes an HTTP request to extract and handle user claims from the JWT token.
        /// </summary>
        /// <param name="context">The HttpContext for the current request.</param>
        /// <param name="next">The next middleware delegate in the pipeline.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                var claims = ExtractTokenClaims(context);
                if (claims.Any())
                {
                    context.Items["UserClaims"] = claims;
                }
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Extracts claims from a JWT token in the Authorization header.
        /// </summary>
        /// <param name="context">The HttpContext containing the request headers.</param>
        /// <returns>A dictionary of claim types and values. Returns empty dictionary if no valid token is found.</returns>
        private static IDictionary<string, string> ExtractTokenClaims(HttpContext context)
        {
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return new Dictionary<string, string>(); // Trả về danh sách rỗng nếu không có token
            }

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
            {
                return new Dictionary<string, string>(); // Trả về danh sách rỗng nếu token không hợp lệ
            }

            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.ToDictionary(c => c.Type, c => c.Value);
        }
    }
}
