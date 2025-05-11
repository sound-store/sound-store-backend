using System.Diagnostics.CodeAnalysis;

namespace SoundStore.Core.Commons
{
    [ExcludeFromCodeCoverage]
    public sealed class PayOSSettings
    {
        public string ClientId { get; init; } = null!;

        public string ApiKey { get; init; } = null!;

        public string ChecksumKey { get; init; } = null!;
    }
}
