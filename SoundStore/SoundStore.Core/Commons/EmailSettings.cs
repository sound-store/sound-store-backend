using System.Diagnostics.CodeAnalysis;

namespace SoundStore.Core.Commons
{
    [ExcludeFromCodeCoverage]
    public sealed class EmailSettings
    {
        public string SmtpServer { get; init; } = null!;

        public int Port { get; init; }

        public string SenderEmail { get; init; } = null!;
        
        public string SenderName { get; init; } = null!;

        public string Password { get; init; } = null!;
    }
}
