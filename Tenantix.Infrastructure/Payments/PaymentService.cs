using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Domain.Entities;
using Tenantix.Domain.Enums;
using Tenantix.Infrastructure.Persistence.Context;

namespace Tenantix.Infrastructure.Payments
{
    public class PaymentService : IPaymentService
        
    {
        private readonly ApplicationDbContext _context;

        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateAsync(Guid orderId, PaymentProvider provider, CancellationToken cancellationToken)
        {
                 var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

                var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.IsActive ,cancellationToken);
                if(order is  null)
                    throw new InvalidOperationException($"Order with ID {orderId} not found or is inactive.");
                if(order.PaymentStatus == OrderPaymentStatus.Paid)
                    throw new InvalidOperationException($"Order with ID {orderId} is already paid.");
                var payment = new Payment(
                    order.Id,
                    order.TotalAmount,
                    "EGP",
                    provider
                );
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
                return payment.Id;
                });

        }

        public async Task<bool> MarkAsPaidAsync(Guid paymentId, CancellationToken cancellationToken)
        {
            var payment = await _context.Payments
               .Include(p => p.Order)
               .FirstOrDefaultAsync(p => p.Id == paymentId && p.IsActive, cancellationToken);

            if (payment is null)
                return false;

            payment.MarkAsPaid();
            payment.Order.MarkAsPaid();

            await _context.SaveChangesAsync(cancellationToken);
            return true; ;
        }
        public async Task<bool> MarkAsFailedAsync(Guid paymentId, CancellationToken cancellationToken)
        {
            var payment = await _context.Payments
               .Include(p => p.Order)
               .FirstOrDefaultAsync(p => p.Id == paymentId && p.IsActive, cancellationToken);
            if(payment is null)  return false;
            payment.MarkAsFailed();
            payment.Order.MarkAsPaymentFailed();
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
