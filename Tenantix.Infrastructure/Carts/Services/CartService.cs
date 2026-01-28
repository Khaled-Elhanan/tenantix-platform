using Microsoft.EntityFrameworkCore;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Carts.DTOs;
using Tenantix.Domain.Entities;
using Tenantix.Infrastructure.Persistence.Context;
using Tenantix.Shared.Exceptions;

namespace Tenantix.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;
    private readonly IProductService _productService;

    public CartService(ApplicationDbContext context, IProductService productService)
    {
        _context = context;
        _productService = productService;
    }

    public async Task<CartResponse?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .AsNoTracking()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);

        if (cart is null)
            return null;

        return new CartResponse
        {
            CustomerId = cart.CustomerId,
            Items = cart.Items.Select(i => new CartItemResponse
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }

    public async Task AddItemAsync(Guid customerId, AddCartItemRequest item, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);

        if (cart is null)
        {
            cart = new Cart { CustomerId = customerId };
            _context.Carts.Add(cart);
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);

        // Always load product (we need price + stock)
        var product = await _productService.GetByIdAsync(item.ProductId, cancellationToken);
        if (product is null)
            throw new NotFoundException(new List<string> { "Product not found." });

        if (existingItem is null)
        {
            // Stock check for new item
            if (product.Stock < item.Quantity)
                throw new ConflictException(new List<string> { "Not enough stock for this product." });

            cart.Items.Add(new CartItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }
        else
        {
            // Stock check for increased quantity
            var newQty = existingItem.Quantity + item.Quantity;

            if (product.Stock < newQty)
                throw new ConflictException(new List<string> { "Not enough stock for this product." });

            existingItem.Quantity = newQty;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateItemAsync(Guid customerId, UpdateCartItemRequest item, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);

        if (cart is null)
            throw new NotFoundException(new List<string> { "Cart not found." });

        var existing = cart.Items.FirstOrDefault(x => x.ProductId == item.ProductId);

        if (existing is null)
            throw new NotFoundException(new List<string> { "Cart item not found." });

        // quantity <= 0 => remove item
        if (item.Quantity <= 0)
        {
            cart.Items.Remove(existing);
            await _context.SaveChangesAsync(cancellationToken);
            return;
        }

        // check product exists + stock
        var product = await _productService.GetByIdAsync(item.ProductId, cancellationToken);
        if (product is null)
            throw new NotFoundException(new List<string> { "Product not found." });

        if (product.Stock < item.Quantity)
            throw new ConflictException(new List<string> { "Not enough stock for this product." });

        existing.Quantity = item.Quantity;

        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task ClearAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
        .Include(x => x.Items)
        .FirstOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);

        if (cart is null)
            throw new NotFoundException(new List<string> { "Cart not found." });

        cart.Items.Clear();

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveItemAsync(Guid customerId, Guid productId, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);
        if (cart is null)
            throw new NotFoundException(new List<string> { "Cart not found." });
        var existing = cart.Items.FirstOrDefault(x => x.ProductId == productId);
        if (existing is null)
            throw new NotFoundException(new List<string> { "Cart item not found." });
        cart.Items.Remove(existing);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<CartSummaryResponse> GetSummaryAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .AsNoTracking()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);

        if (cart is null)
            throw new NotFoundException(new List<string> { "Cart not found." });

        int totalItems = cart.Items.Sum(i => i.Quantity);

        var productIds = cart.Items.Select(i => i.ProductId).Distinct().ToList();

        var prices = await _productService.GetPricesByIdsAsync(productIds, cancellationToken);

        decimal subtotal = 0m;

        foreach (var item in cart.Items)
        {
            if (!prices.TryGetValue(item.ProductId, out var price))
                throw new NotFoundException(new List<string> { "Product not found." });

            subtotal += price * item.Quantity;
        }

        return new CartSummaryResponse
        {
            CustomerId = customerId,
            TotalItems = totalItems,
            SubTotal = subtotal
        };
    }



}
