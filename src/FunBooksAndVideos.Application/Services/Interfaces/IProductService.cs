using FunBooksAndVideos.Application.Dtos;

namespace FunBooksAndVideos.Application.Services.Interfaces;

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
