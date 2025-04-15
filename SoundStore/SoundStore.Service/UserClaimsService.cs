using Microsoft.AspNetCore.Http;

namespace SoundStore.Service
{
    public sealed class UserClaimsService(IHttpContextAccessor httpContextAccessor)
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public string GetClaim(string key)
        {
            var claims = GetClaims();
            return claims.TryGetValue(key, out var value) ? value : string.Empty;
        }

        private IDictionary<string, string> GetClaims() 
            => _httpContextAccessor.HttpContext?.Items["UserClaims"] 
                as IDictionary<string, string> ?? new Dictionary<string, string>();
    }
}
