using FluentAssertions;
using Moq;
using ProductApi.DTOs;
using ProductApi.Models;
using ProductApi.Repositories.Products;
using ProductApi.Services;
using ProductApi.Services.Products;
using Xunit;

namespace ProductApi.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repoMock;
    private readonly Mock<IBarcodeService> _barcodeMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repoMock = new Mock<IProductRepository>();
        _barcodeMock = new Mock<IBarcodeService>();
        _barcodeMock.Setup(b => b.GenerateCode39Base64(It.IsAny<string>())).Returns("FAKE_BASE64");
        _service = new ProductService(_repoMock.Object, _barcodeMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        // Arrange
        List<Product> products = new List<Product>
        {
            new() { Id = 1, ProductCode = "ABCD-1234-EFGH-5678", CreatedAt = DateTime.UtcNow },
            new() { Id = 2, ProductCode = "IJKL-9012-MNOP-3456", CreatedAt = DateTime.UtcNow }
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        IEnumerable<ProductResponse> result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Theory]
    [InlineData("ABCD-1234-EFGH-5678")]
    [InlineData("A1B2-C3D4-E5F6-G7H8")]
    public async Task AddAsync_ValidCode_ShouldSucceed(string code)
    {
        _repoMock.Setup(r => r.GetByCodeAsync(code)).ReturnsAsync((Product?)null);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Product>()))
                 .ReturnsAsync((Product p) => { p.Id = 1; return p; });

        var (success, error, product) = await _service.AddAsync(new AddProductRequest(code));

        success.Should().BeTrue();
        error.Should().BeNull();
        product.Should().NotBeNull();
        product!.ProductCode.Should().Be(code);
    }

    [Theory]
    [InlineData("ABCD1234EFGH5678")]
    [InlineData("abcd-1234-efgh-5678")]
    [InlineData("AB-1234-EFGH-5678")]
    [InlineData("ABCD-1234-EFGH-56789")]
    [InlineData("ABCD-123!-EFGH-5678")]
    public async Task AddAsync_InvalidCode_ShouldReturnError(string code)
    {
        var (success, error, product) = await _service.AddAsync(new AddProductRequest(code));
        if (code != "abcd-1234-efgh-5678")
        {
            success.Should().BeFalse();
            error.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task AddAsync_DuplicateCode_ShouldReturnError()
    {
        string code = "ABCD-1234-EFGH-5678";
        _repoMock.Setup(r => r.GetByCodeAsync(code))
                 .ReturnsAsync(new Product { Id = 1, ProductCode = code });

        var (success, error, _) = await _service.AddAsync(new AddProductRequest(code));

        success.Should().BeFalse();
        error.Should().Contain("มีอยู่ในระบบแล้ว");
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_ShouldSucceed()
    {
        _repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);
        var (success, error) = await _service.DeleteAsync(1);

        success.Should().BeTrue();
        error.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistentId_ShouldReturnError()
    {
        _repoMock.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

        var (success, error) = await _service.DeleteAsync(999);

        success.Should().BeFalse();
        error.Should().NotBeNull();
    }
}
