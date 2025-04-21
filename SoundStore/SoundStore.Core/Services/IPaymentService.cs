namespace SoundStore.Core.Services
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentLinkPayOS(long transactionId);
    }
}
