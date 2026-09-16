using FunBooksAndVideos.Application.Processing;
using FunBooksAndVideos.Domain.Customers;
using FunBooksAndVideos.Domain.Memberships;
using FunBooksAndVideos.Domain.Products;
using FunBooksAndVideos.Domain.PurchaseOrders;
using FunBooksAndVideos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FunBooksAndVideos.Tests;

internal static class TestData
{
    public static Customer Customer(MembershipType membership = MembershipType.None)
    {
        return new Customer("Jane Doe", "jane.doe@example.com") { Membership = membership };
    }

    public static Book Book()
    {
        return new Book("The Girl on the Train", 14.50m, "Paula Hawkins") { Id = 1 };
    }

    public static Video Video()
    {
        return new Video("Comprehensive First Aid Training", 19.00m, 95) { Id = 2 };
    }

    public static Membership Membership(MembershipType type = MembershipType.BookClub)
    {
        return new Membership($"{type} Membership", 15.00m, type) { Id = 3 };
    }

    public static PurchaseOrderProcessingContext Context(Customer customer, params Product[] products)
    {
        var order = new PurchaseOrder
        {
            CustomerId = customer.Id,
            Lines = products
                .Select(product => new PurchaseOrderLine
                {
                    ProductId = product.Id,
                    Product = product,
                    ProductName = product.Name,
                    Price = product.Price
                })
                .ToList()
        };

        return new PurchaseOrderProcessingContext { Order = order, Customer = customer };
    }

    public static AppDbContext DbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"Tests-{Guid.NewGuid()}")
            .Options;

        return new AppDbContext(options);
    }
}
