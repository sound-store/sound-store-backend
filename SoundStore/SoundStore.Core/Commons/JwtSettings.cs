namespace SoundStore.Core.Commons
{
    public sealed class JwtSettings
    {
        public string Issuer { get; init; } = null!;

        public string Audience { get; init; } = null!;

        public string Key { get; init; } = null!;
    }
}
