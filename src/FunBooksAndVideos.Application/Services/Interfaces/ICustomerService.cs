using FunBooksAndVideos.Application.Dtos;

namespace FunBooksAndVideos.Application.Services.Interfaces;

public interface ICustomerService
{
    Task<List<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
