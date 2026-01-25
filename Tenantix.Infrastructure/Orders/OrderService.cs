using Microsoft.EntityFrameworkCore;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Orders.DTOs;
using Tenantix.Domain.Entities;
using Tenantix.Domain.Enums;
using Tenantix.Infrastructure.Persistence.Context;
using Tenantix.Shared.Models;

namespace Tenantix.Infrastructure.Orders
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

      

        public async Task<Guid> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            // 1) validate customer exists
            var customerExsit = await _context.Customers.
                AnyAsync(c => c.Id == request.CustomerId && c.IsActive, cancellationToken);
            if (!customerExsit)
            {
                throw new InvalidOperationException("Customer not found.");
            }
            // 2) load products
            var productIds = request.OrderItems.Select(x => x.ProductId).Distinct().ToList();
            var products = await _context.Products.Where(p => productIds.Contains(p.Id) && p.IsActive)
                .ToListAsync(cancellationToken);
            if (products.Count != productIds.Count)
            {
                throw new InvalidOperationException("One or more prodcuts not found .");

            }
            // 3) stock check 

            foreach (var item in request.OrderItems)
            {
                if (item.Quantity <= 0)
                {
                    throw new InvalidOperationException("Quantity must be greater than zero.");
                }
                var product = products.First(p => p.Id == item.ProductId);
                if (product.Stock < item.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for product: {product.Name}");
                }
            }
            // 4) create order

            var order = new Order
            {
                CustomerId = request.CustomerId,
                Notes = request.Notes,
                AddressLine = request.AddressLine,
                City = request.City,
                Phone = request.Phone,
                OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}",
            };
            decimal total = 0;
            foreach (var item in request.OrderItems)
            {
                var product = products.First(p => p.Id == item.ProductId);

                var unitPrice = product.Price;
                var lineTotal = unitPrice * item.Quantity;

                total += lineTotal;

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = unitPrice,
                    Quantity = item.Quantity,
                    LineTotal = lineTotal
                });

                product.Stock -= item.Quantity;
            }

            order.TotalAmount = total;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            return order.Id;

        }

        public async Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Orders.AsNoTracking()
                .Where(o => o.Id == id && o.IsActive)
                .Select(o => new OrderResponse
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    TotalAmount = o.TotalAmount,
                    OrderNumber=o.OrderNumber,
                    Status = o.Status.ToString(),
                    CreateAt = o.CreatedAt,
                    Notes = o.Notes,
                    AddressLine = o.AddressLine,
                    City = o.City,
                    Phone = o.Phone,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemResponse
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        LineTotal = oi.LineTotal
                    }).ToList()
                   
                }).FirstOrDefaultAsync(cancellationToken);

        }                                           

        public async Task<PagedResponse<OrderListItemResponse>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Orders.AsNoTracking()
                .Where(p => p.IsActive)
                .OrderByDescending(o => o.CreatedAt);

            var totalCount =await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderListItemResponse
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerId = o.CustomerId,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status.ToString(),
                    CreateAt = o.CreatedAt
                })
                .ToListAsync(cancellationToken);
            return new PagedResponse<OrderListItemResponse>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

        }
        public async Task<bool> CancelAsync(Guid id, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id , cancellationToken);
            if(order is null) return false;
            if(order.Status == OrderStatus.Cancelled) return false;
            // restore stock 
            var productIds = order.OrderItems.Select(oi => oi.ProductId).ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
            foreach(var item in order.OrderItems)
            {
                var product = products.First(p => p.Id == item.ProductId);
                product.Stock += item.Quantity;
            }
            // cancel order 
            order.Status= OrderStatus.Cancelled;
            await _context.SaveChangesAsync(cancellationToken);
            return true;

        }

        public async Task<bool> UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.
                FirstOrDefaultAsync(o => o.Id == id , cancellationToken);
            if (order is null) return false;
            // prevent changes on delivered or cancelled orders 
            if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Cannot change status of a completed or cancelled order");
            // validate allowed transitions
            var isValidTransition = order.Status switch
            {
                OrderStatus.Pending => status == OrderStatus.Confirmed || status == OrderStatus.Cancelled,
                // storefront order waiting for payment
                OrderStatus.PendingPayment=> status == OrderStatus.Confirmed || status == OrderStatus.Cancelled,
                OrderStatus.Confirmed => status == OrderStatus.Shipped || status == OrderStatus.Cancelled,
                OrderStatus.Shipped => status == OrderStatus.Delivered,
                _ => false
            };
            if (!isValidTransition)
            {
                throw new InvalidOperationException($"Invalid status transition from {order.Status} to {status}");
            }

            order.Status = status;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
