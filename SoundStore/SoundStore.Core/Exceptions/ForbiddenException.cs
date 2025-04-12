namespace SoundStore.Core.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message)
        {
        }
        public ForbiddenException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
