using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FunBooksAndVideos.Application.Dtos;
using FunBooksAndVideos.Domain.Customers;
using FunBooksAndVideos.Domain.Memberships;
using FunBooksAndVideos.Domain.Products;
using FunBooksAndVideos.Domain.PurchaseOrders;
using FunBooksAndVideos.Infrastructure.Persistence;
using FunBooksAndVideos.Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FunBooksAndVideos.Tests.Api;

public class PurchaseOrdersControllerTests : IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    private readonly Book _book = new("The Girl on the Train", 14.50m, "Paula Hawkins");
    private readonly Video _video = new("Comprehensive First Aid Training", 19.00m, 95);
    private readonly Membership _bookClub = new("Book Club Membership", 15.00m, MembershipType.BookClub);
    private readonly Membership _videoClub = new("Video Club Membership", 15.00m, MembershipType.VideoClub);
    private readonly Customer _jane = new("Jane Doe", "jane.doe@example.com");
    private readonly Customer _john = new("John Smith", "john.smith@example.com");

    public PurchaseOrdersControllerTests()
    {
        var databaseName = $"Tests-{Guid.NewGuid()}";
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
                services.AddDbContext<AppDbContext>((serviceProvider, options) => options
                    .UseInMemoryDatabase(databaseName)
                    .AddInterceptors(serviceProvider.GetRequiredService<AuditInterceptor>()));
            }));
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Products.AddRangeAsync(_book, _video, _bookClub, _videoClub);
        await dbContext.Customers.AddRangeAsync(_jane, _john);
        await dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task KataExample_ActivatesMembership_AndGeneratesShippingSlip()
    {
        var request = new CreatePurchaseOrderRequest
        {
            CustomerId = _jane.Id,
            ProductIds = [_video.Id, _book.Id, _bookClub.Id]
        };

        var response = await _client.PostAsJsonAsync("api/purchase-orders", request, JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var order = await ReadAsync<PurchaseOrderDto>(response);
        Assert.Equal(48.50m, order.Total);
        Assert.Equal(PurchaseOrderStatus.Processed, order.Status);
        Assert.Equal(3, order.Lines.Count);
        Assert.Equal($"/api/purchase-orders/{order.Id}", response.Headers.Location?.AbsolutePath);

        var customer = await GetAsync<CustomerDto>($"api/customers/{_jane.Id}");
        Assert.Equal(MembershipType.BookClub, customer.Membership);

        var slip = await GetAsync<ShippingSlipDto>($"api/purchase-orders/{order.Id}/shipping-slip");
        Assert.Equal(order.Id, slip.PurchaseOrderId);
        Assert.Equal(_book.Name, Assert.Single(slip.Lines).ProductName);

        var storedOrder = await GetAsync<PurchaseOrderDto>($"api/purchase-orders/{order.Id}");
        Assert.Equal(["Video", "Book", "Membership"], storedOrder.Lines.Select(line => line.ProductType));
    }

    [Fact]
    public async Task BuyingBothClubs_MakesCustomerPremium()
    {
        var request = new CreatePurchaseOrderRequest
        {
            CustomerId = _john.Id,
            ProductIds = [_bookClub.Id, _videoClub.Id]
        };

        var response = await _client.PostAsJsonAsync("api/purchase-orders", request, JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var customer = await GetAsync<CustomerDto>($"api/customers/{_john.Id}");
        Assert.Equal(MembershipType.Premium, customer.Membership);
    }

    [Fact]
    public async Task Products_HaveAuditColumnsFilledIn()
    {
        var products = await GetAsync<List<ProductDto>>("api/products");

        Assert.Equal(4, products.Count);
        Assert.All(products, product =>
        {
            Assert.Equal("System", product.CreatedBy);
            Assert.NotEqual(default, product.CreatedAt);
        });
    }

    [Fact]
    public async Task DigitalOnlyOrder_HasNoShippingSlip()
    {
        var request = new CreatePurchaseOrderRequest { CustomerId = _jane.Id, ProductIds = [_video.Id] };

        var response = await _client.PostAsJsonAsync("api/purchase-orders", request, JsonOptions);
        var order = await ReadAsync<PurchaseOrderDto>(response);

        var slipResponse = await _client.GetAsync($"api/purchase-orders/{order.Id}/shipping-slip");
        Assert.Equal(HttpStatusCode.NotFound, slipResponse.StatusCode);
    }

    [Fact]
    public async Task InvalidRequest_Returns400()
    {
        var response = await _client.PostAsJsonAsync("api/purchase-orders", new CreatePurchaseOrderRequest(), JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UnknownCustomer_Returns404()
    {
        var request = new CreatePurchaseOrderRequest { CustomerId = 999, ProductIds = [_book.Id] };

        var response = await _client.PostAsJsonAsync("api/purchase-orders", request, JsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UnknownProduct_Returns404()
    {
        var request = new CreatePurchaseOrderRequest { CustomerId = _jane.Id, ProductIds = [999] };

        var response = await _client.PostAsJsonAsync("api/purchase-orders", request, JsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UnknownPurchaseOrder_Returns404()
    {
        var response = await _client.GetAsync("api/purchase-orders/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<T> GetAsync<T>(string url)
    {
        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await ReadAsync<T>(response);
    }

    private static async Task<T> ReadAsync<T>(HttpResponseMessage response)
    {
        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        return result!;
    }
}
