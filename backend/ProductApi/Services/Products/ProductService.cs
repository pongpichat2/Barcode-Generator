using ProductApi.DTOs;
using ProductApi.Models;
using ProductApi.Repositories.Products;
using System.Text.RegularExpressions;

namespace ProductApi.Services.Products;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;
    private readonly IBarcodeService _barcodeService;

    // Format: xxxx-xxxx-xxxx-xxxx  (16 alphanumeric uppercase + 3 dashes = 19 chars)
    private static readonly Regex CodeRegex = new(@"^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$");

    public ProductService(IProductRepository repo, IBarcodeService barcodeService)
    {
        _repo = repo;
        _barcodeService = barcodeService;
    }

    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var products = await _repo.GetAllAsync();
        return products.Select(p => MapToResponse(p));
    }

    public async Task<(bool Success, string? Error, ProductResponse? Product)> AddAsync(AddProductRequest request)
    {
        var code = request.ProductCode.ToUpperInvariant().Trim();

        if (!CodeRegex.IsMatch(code))
            return (false, "รหัสสินค้าต้องเป็นรูปแบบ xxxx-xxxx-xxxx-xxxx (ตัวเลขและตัวอักษรภาษาอังกฤษพิมพ์ใหญ่เท่านั้น)", null);

        var existing = await _repo.GetByCodeAsync(code);
        if (existing is not null)
            return (false, "รหัสสินค้านี้มีอยู่ในระบบแล้ว", null);

        var product = new Product { ProductCode = code };
        var saved = await _repo.AddAsync(product);
        return (true, null, MapToResponse(saved));
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        Product? product = await _repo.GetByIdAsync(id);
        if (product == null) return (false, "ไม่พบสินค้าที่ต้องการลบ");
        product.DeleteAt = DateTime.UtcNow;
        await _repo.UpdateAsync(product);
        return (true, null);
    }

    private ProductResponse MapToResponse(Product p)
    {
        var barcode = _barcodeService.GenerateCode39Base64(p.ProductCode.Replace("-", ""));
        return new ProductResponse(p.Id, p.ProductCode, barcode, p.CreatedAt);
    }
}
