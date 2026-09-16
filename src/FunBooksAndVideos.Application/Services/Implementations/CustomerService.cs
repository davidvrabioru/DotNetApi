using FunBooksAndVideos.Application.Dtos;
using FunBooksAndVideos.Application.Services.Interfaces;
using FunBooksAndVideos.Domain.Customers;
using FunBooksAndVideos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FunBooksAndVideos.Application.Services.Implementations;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _dbContext;

    public CustomerService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _dbContext.Customers
            .AsNoTracking()
            .OrderBy(customer => customer.Id)
            .ToListAsync(cancellationToken);

        return customers.Select(ToDto).ToList();
    }

    public async Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await _dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(customer => customer.Id == id, cancellationToken);

        return customer is null ? null : ToDto(customer);
    }

    private static CustomerDto ToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Membership = customer.Membership
        };
    }
}
