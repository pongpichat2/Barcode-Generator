using ProductApi.DTOs;

namespace ProductApi.Services.Products;

public interface IProductService
{
    Task<IEnumerable<ProductResponse>> GetAllAsync();
    Task<(bool Success, string? Error, ProductResponse? Product)> AddAsync(AddProductRequest request);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}
