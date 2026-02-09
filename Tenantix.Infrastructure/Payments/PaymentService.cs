using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Payments.DTOs;
using Tenantix.Domain.Entities;
using Tenantix.Domain.Enums;
using Tenantix.Infrastructure.Persistence.Context;
using Tenantix.Shared.Models;

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

        public async Task<string> InitiateAsync(Guid paymentId, CancellationToken ct)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == paymentId && p.IsActive, ct);
            if (payment is null)
                throw new InvalidOperationException($"Payment with ID {paymentId} not found or is inactive.");
            if(payment.Status != PaymentStatus.Initialized)
                throw new InvalidOperationException("Payment already initiated.");
            var externalRef = $"PAY-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}";
            var paymentUrl = $"https://mock-payments.tenantix.com/pay/{externalRef}";
            payment.MarkAsPending(externalRef, paymentUrl);
            await _context.SaveChangesAsync(ct);
            return paymentUrl;
        }
        public async Task<bool> RefundAsync(Guid paymentId, CancellationToken ct)
        {
            var payment = await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.IsActive, ct);

            if (payment is null)
                return false;

            payment.Refund();
            payment.Order.MarkAsRefunded();

            await _context.SaveChangesAsync(ct);
            return true;
        }
        public async Task<List<PaymentResponse>> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken ct)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(p => p.OrderId == orderId && p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PaymentResponse
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    Status = p.Status.ToString(),
                    Provider = p.Provider.ToString(),
                    ExternalReference = p.ExternalReference,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync(ct);
        }

        public async Task<PagedResponse<PaymentResponse>> GetPagedAsync(
            int page,
            int pageSize,
            CancellationToken ct)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 200);
            var query = _context.Payments
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt);
            var totalCount  = await query.CountAsync(ct);
            var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(p => new PaymentResponse
        {
            Id = p.Id,
            Amount = p.Amount,
            Status = p.Status.ToString(),
            Provider = p.Provider.ToString(),
            ExternalReference = p.ExternalReference,
            CreatedAt = p.CreatedAt
        })
        .ToListAsync(ct);

            return new PagedResponse<PaymentResponse>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}                                         
