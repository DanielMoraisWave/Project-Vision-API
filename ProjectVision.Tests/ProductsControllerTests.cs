using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectVision.Api.Controllers;
using ProjectVision.Api.Data;

namespace ProjectVision.Tests;

public class ProductsControllerTests
{
    [Fact]
    public async Task AddToStock_WithZeroQuantity_ShouldReturnBadRequest()
    {
        await using var context = CreateContext();
        var controller = new ProductsController(context);

        var result = await controller.AddToStock(100000, 0);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task AddToStock_WithNegativeQuantity_ShouldReturnBadRequest()
    {
        await using var context = CreateContext();
        var controller = new ProductsController(context);

        var result = await controller.AddToStock(100000, -5);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task DecrementStock_WithZeroQuantity_ShouldReturnBadRequest()
    {
        await using var context = CreateContext();
        var controller = new ProductsController(context);

        var result = await controller.DecrementStock(100000, 0);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task DecrementStock_WithNegativeQuantity_ShouldReturnBadRequest()
    {
        await using var context = CreateContext();
        var controller = new ProductsController(context);

        var result = await controller.DecrementStock(100000, -2);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetProduct_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        await using var context = CreateContext();
        var controller = new ProductsController(context);

        var result = await controller.GetProduct(999999);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task SearchProducts_WithEmptyName_ShouldReturnBadRequest()
    {
        await using var context = CreateContext();
        var controller = new ProductsController(context);

        var result = await controller.SearchProducts(string.Empty);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task SearchProducts_WithWhitespaceName_ShouldReturnBadRequest()
    {
        await using var context = CreateContext();
        var controller = new ProductsController(context);

        var result = await controller.SearchProducts("   ");

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetProductsByStockLevel_WhenMinIsGreaterThanMax_ShouldReturnBadRequest()
    {
        await using var context = CreateContext();
        var controller = new ProductsController(context);

        var result = await controller.GetProductsByStockLevel(30, 5);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetProductsByStockLevel_WhenMinIsNegative_ShouldReturnBadRequest()
    {
        await using var context = CreateContext();
        var controller = new ProductsController(context);

        var result = await controller.GetProductsByStockLevel(-1, 20);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetProductsByStockLevel_WhenMaxIsNegative_ShouldReturnBadRequest()
    {
        await using var context = CreateContext();
        var controller = new ProductsController(context);

        var result = await controller.GetProductsByStockLevel(0, -1);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}