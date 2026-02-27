using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.DTOs;
using ProductApi.Services.Products;
using System.Net;

namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try {
            IEnumerable<ProductResponse> products = await _productService.GetAllAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {

            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An error occurred while retrieving products.", details = ex.Message });
        }

    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddProductRequest request)
    {
        try {

            var (success, error, product) = await _productService.AddAsync(request);
            if (!success) return BadRequest(new { message = error });

            return StatusCode((int)HttpStatusCode.Created, product);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An error occurred while adding the product.", details = ex.Message });
        }

    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try {
            var (success, error) = await _productService.DeleteAsync(id);
            if (!success) return NotFound(new { message = error });
            return Ok($"Prodict {id} Deleted");
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An error occurred while deleting the product.", details = ex.Message });
        }

    }
}
