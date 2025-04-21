using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using SoundStore.Core;
using SoundStore.Core.Commons;
using SoundStore.Core.Services;
using Transaction = SoundStore.Core.Entities.Transaction;

namespace SoundStore.Service
{
    public class PaymentService(IUnitOfWork unitOfWork,
        IOptions<PayOSSettings> options,
        ILogger<PaymentService> logger) : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<PaymentService> _logger = logger;
        private readonly PayOSSettings _options = options.Value;

        public async Task<string> CreatePaymentLinkPayOS(long transactionId)
        {
            try
            {
                var transactionRepository = _unitOfWork.GetRepository<Transaction>();
                PayOS payOS = new PayOS(_options.ClientId, _options.ApiKey, _options.ChecksumKey);
                List<ItemData> items = new List<ItemData>();

                var transaction = await transactionRepository.GetAll()
                    .AsNoTracking()
                    .Include(t => t.Order)
                    .ThenInclude(o => o.OrderDetails)
                    .FirstOrDefaultAsync(t => t.Id == transactionId) 
                        ?? throw new KeyNotFoundException("Transaction does not exist!");
                //ItemData
                // Add item into items to create payment data

                //if (!isExisted)
                //    throw new KeyNotFoundException("Transaction does not exist!");
                //var paymentData = new PaymentData();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                throw;
            }

            throw new NotImplementedException();
        }
    }
}
