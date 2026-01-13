using Mapster;
using Microsoft.EntityFrameworkCore;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Customers.DTOs;
using Tenantix.Domain.Entities;
using Tenantix.Infrastructure.Persistence.Context;
using Tenantix.Shared.Models;

namespace Tenantix.Infrastructure.Customers.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;
        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken)
        {
            var customer = request.Adapt<Customer>();
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync(cancellationToken);
            return customer.Id;

        }

     

        public async Task<CustomerResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Customers
          .AsNoTracking()
          .Where(x => x.Id == id && x.IsActive)
          .Select(x => new CustomerResponse
          {
              Id = x.Id,
              FirstName = x.FirstName,
              LastName = x.LastName,
              Email = x.Email,
              Phone = x.Phone,
              IsActive = x.IsActive,
              CreatedAt = x.CreatedAt
          })
          .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PagedResponse<CustomerListItemResponse>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Customers.AsNoTracking()
                .Where(x => x.IsActive);
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new CustomerListItemResponse
                {
                    Id = x.Id,
                    FullName = $"{x.FirstName} {x.LastName}",
                    Email = x.Email,
                    Phone = x.Phone

                }).ToListAsync(cancellationToken);
            return new PagedResponse<CustomerListItemResponse>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

        }

        public async Task<bool> UpdateAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (customer == null)
            {
                return false;
            }
            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;
            customer.Phone = request.Phone;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
           

        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
           var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (customer == null)
            {
                return false;
            }
            customer.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

       
    }
    }
