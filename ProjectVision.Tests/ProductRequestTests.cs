using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using ProjectVision.Api.DTOs;

namespace ProjectVision.Tests;

public class ProductRequestTests
{
    [Fact]
    public void ValidRequest_ShouldPassValidation()
    {
        var request = new ProductRequest
        {
            Name = "Laptop",
            Description = "Gaming laptop",
            Price = 999.99m,
            Stock = 10
        };

        var results = Validate(request);

        results.Should().BeEmpty();
    }

    [Fact]
    public void EmptyName_ShouldFailValidation()
    {
        var request = new ProductRequest
        {
            Name = string.Empty,
            Price = 100,
            Stock = 10
        };

        var results = Validate(request);

        results.Should().Contain(result =>
            result.MemberNames.Contains(nameof(ProductRequest.Name)));
    }

    [Fact]
    public void NegativePrice_ShouldFailValidation()
    {
        var request = new ProductRequest
        {
            Name = "Laptop",
            Price = -10,
            Stock = 10
        };

        var results = Validate(request);

        results.Should().Contain(result =>
            result.MemberNames.Contains(nameof(ProductRequest.Price)));
    }

    [Fact]
    public void ZeroPrice_ShouldFailValidation()
    {
        var request = new ProductRequest
        {
            Name = "Laptop",
            Price = 0,
            Stock = 10
        };

        var results = Validate(request);

        results.Should().Contain(result =>
            result.MemberNames.Contains(nameof(ProductRequest.Price)));
    }

    [Fact]
    public void NegativeStock_ShouldFailValidation()
    {
        var request = new ProductRequest
        {
            Name = "Laptop",
            Price = 100,
            Stock = -1
        };

        var results = Validate(request);

        results.Should().Contain(result =>
            result.MemberNames.Contains(nameof(ProductRequest.Stock)));
    }

    [Fact]
    public void DescriptionOver500Characters_ShouldFailValidation()
    {
        var request = new ProductRequest
        {
            Name = "Laptop",
            Description = new string('A', 501),
            Price = 100,
            Stock = 10
        };

        var results = Validate(request);

        results.Should().Contain(result =>
            result.MemberNames.Contains(nameof(ProductRequest.Description)));
    }

    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(
            model,
            new ValidationContext(model),
            results,
            validateAllProperties: true);

        return results;
    }
}