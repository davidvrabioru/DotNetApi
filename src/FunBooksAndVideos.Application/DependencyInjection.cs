using FunBooksAndVideos.Application.Processing.Implementations;
using FunBooksAndVideos.Application.Processing.Implementations.Rules;
using FunBooksAndVideos.Application.Processing.Interfaces;
using FunBooksAndVideos.Application.Services.Implementations;
using FunBooksAndVideos.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FunBooksAndVideos.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();

        services.AddScoped<IPurchaseOrderProcessor, PurchaseOrderProcessor>();
        services.AddScoped<IPurchaseOrderRule, ActivateMembershipRule>();
        services.AddScoped<IPurchaseOrderRule, GenerateShippingSlipRule>();

        return services;
    }
}
