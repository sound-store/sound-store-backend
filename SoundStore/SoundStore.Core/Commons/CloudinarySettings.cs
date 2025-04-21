namespace SoundStore.Core.Commons
{
    public sealed class CloudinarySettings
    {
        public string CloudName { get; init; } = null!;

        public string ApiKey { get; set; } = null!;
        
        public string ApiSecret { get; set; } = null!;
    }
}
