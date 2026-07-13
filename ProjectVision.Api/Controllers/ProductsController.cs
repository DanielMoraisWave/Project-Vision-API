using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectVision.Api.Data;
using ProjectVision.Api.DTOs;
using ProjectVision.Api.Models;

namespace ProjectVision.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await _context.Products
            .AsNoTracking()
            .OrderBy(product => product.Id)
            .ToListAsync();

        return Ok(products);
    }

    // GET /api/products/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(product => product.Id == id);

        if (product is null)
        {
            return NotFound(new
            {
                message = "Product not found."
            });
        }

        return Ok(product);
    }

    // POST /api/products
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(
        ProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price,
            Stock = request.Stock
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = product.Id },
            product);
    }

    // PUT /api/products/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Product>> UpdateProduct(
        int id,
        ProductRequest request)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(product => product.Id == id);

        if (product is null)
        {
            return NotFound(new
            {
                message = "Product not found."
            });
        }

        product.Name = request.Name.Trim();
        product.Description = request.Description?.Trim();
        product.Price = request.Price;
        product.Stock = request.Stock;

        await _context.SaveChangesAsync();

        return Ok(product);
    }

    // DELETE /api/products/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var deletedRows = await _context.Products
            .Where(product => product.Id == id)
            .ExecuteDeleteAsync();

        if (deletedRows == 0)
        {
            return NotFound(new
            {
                message = "Product not found."
            });
        }

        return NoContent();
    }

    // POST /api/products/{id}/decrement-stock/{quantity}
    [HttpPost("{id:int}/decrement-stock/{quantity:int}")]
    public async Task<IActionResult> DecrementStock(
        int id,
        int quantity)
    {
        if (quantity <= 0)
        {
            return BadRequest(new
            {
                message = "Quantity must be greater than zero."
            });
        }

        var updatedRows = await _context.Products
            .Where(product =>
                product.Id == id &&
                product.Stock >= quantity)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(
                    product => product.Stock,
                    product => product.Stock - quantity));

        if (updatedRows == 1)
        {
            return Ok(new
            {
                message = "Stock decremented successfully."
            });
        }

        var productExists = await _context.Products
            .AnyAsync(product => product.Id == id);

        if (!productExists)
        {
            return NotFound(new
            {
                message = "Product not found."
            });
        }

        return Conflict(new
        {
            message = "Insufficient stock."
        });
    }

    // POST /api/products/{id}/add-to-stock/{quantity}
    [HttpPost("{id:int}/add-to-stock/{quantity:int}")]
    public async Task<IActionResult> AddToStock(
        int id,
        int quantity)
    {
        if (quantity <= 0)
        {
            return BadRequest(new
            {
                message = "Quantity must be greater than zero."
            });
        }

        var updatedRows = await _context.Products
            .Where(product => product.Id == id)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(
                    product => product.Stock,
                    product => product.Stock + quantity));

        if (updatedRows == 0)
        {
            return NotFound(new
            {
                message = "Product not found."
            });
        }

        return Ok(new
        {
            message = "Stock incremented successfully."
        });
    }

    // GET /api/products/search?name={name}
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Product>>> SearchProducts(
        [FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new
            {
                message = "A product name is required."
            });
        }

        var searchTerm = name.Trim();

        var products = await _context.Products
            .AsNoTracking()
            .Where(product =>
                product.Name.Contains(searchTerm))
            .OrderBy(product => product.Name)
            .ToListAsync();

        return Ok(products);
    }

    // GET /api/products/stock-level?min={min}&max={max}
    [HttpGet("stock-level")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByStockLevel(
        [FromQuery] int min,
        [FromQuery] int max)
    {
        if (min < 0 || max < 0)
        {
            return BadRequest(new
            {
                message = "Stock values cannot be negative."
            });
        }

        if (min > max)
        {
            return BadRequest(new
            {
                message = "Minimum stock cannot exceed maximum stock."
            });
        }

        var products = await _context.Products
            .AsNoTracking()
            .Where(product =>
                product.Stock >= min &&
                product.Stock <= max)
            .OrderBy(product => product.Stock)
            .ToListAsync();

        return Ok(products);
    }
}