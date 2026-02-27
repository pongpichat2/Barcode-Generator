using ProductApi.Models;

namespace ProductApi.Repositories.Products;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetByCodeAsync(string code);
    Task<Product> AddAsync(Product product);
    Task<bool> DeleteAsync(int id);
    Task<Product> UpdateAsync(Product product);
}
