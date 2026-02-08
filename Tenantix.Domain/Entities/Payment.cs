using Tenantix.Domain.Common;
using Tenantix.Domain.Enums;

namespace Tenantix.Domain.Entities
{
    public class Payment : AuditableEntity
    {
        private Payment() { }
        public Payment(
         Guid orderId,
         decimal amount,
         string currency,
         PaymentProvider provider)
        {
            OrderId = orderId;
            Amount = amount;
            Currency = currency;
            Provider = provider;
            Status = PaymentStatus.Initialized;
            IsActive = true;
        }
        public Guid OrderId { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; } = default!;
        public PaymentProvider Provider { get; private set; }
        public PaymentStatus Status { get; private set; }

        public string? ExternalReference { get; private set; }
        public string? PaymentUrl { get; private set; }


        public Order Order { get; private set; } = default!;
    

    #region Domain Methods 

        public void MarkAsPending(string externalReference, string? paymentUrl)
        {
            Status = PaymentStatus.Pending;
            ExternalReference = externalReference;
            PaymentUrl = paymentUrl;
        }
        public void MarkAsPaid()
        {
            Status = PaymentStatus.Paid;
        }
        public void MarkAsFailed()

        {
            Status = PaymentStatus.Failed;
        }                                                                                                                 
        public void Refund()
        {
            Status = PaymentStatus.Refunded;
        }
        #endregion
    } }
