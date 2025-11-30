using OMSBlazor.Interfaces.ApplicationServices;
using OMSBlazor.Northwind.OrderAggregate;
using Shouldly;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace OMSBlazor.ApplicationServices;

public class ProductApplicationServiceTests : OMSBlazorApplicationTestBase
{
    private readonly IProductApplicationService _productApplicationService;
    private readonly IRepository<Product, int> _productRepository;
    private readonly IRepository<Category, int> _categoryRepository;

    public ProductApplicationServiceTests()
    {
        _productApplicationService = GetRequiredService<IProductApplicationService>();
        _productRepository = GetRequiredService<IRepository<Product, int>>();
        _categoryRepository = GetRequiredService<IRepository<Category, int>>();
    }

    [Fact]
    public async Task GetProductsAsync_Should_Return_Persisted_Products()
    {
        const int categoryId = 9999;
        const int productId = 10001;

        await WithUnitOfWorkAsync(async () =>
        {
            var category = CreateCategory(categoryId, "Beverages");
            await _categoryRepository.InsertAsync(category);

            var product = CreateProduct(productId, "Chai", category.Id);
            await _productRepository.InsertAsync(product, autoSave: true);
        });

        var products = await _productApplicationService.GetProductsAsync();

        products.ShouldContain(x => x.ProductId == productId && x.ProductName == "Chai");
    }

    private static Category CreateCategory(int id, string name)
    {
        var category = (Category)Activator.CreateInstance(typeof(Category), true)!;
        typeof(Category).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(category, id);
        category.SetCategoryName(name);
        return category;
    }

    private static Product CreateProduct(int id, string name, int categoryId)
    {
        var product = (Product)Activator.CreateInstance(typeof(Product), true)!;
        typeof(Product).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(product, id);
        typeof(Product).GetProperty("CategoryId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(product, categoryId);
        product.SetProductName(name);
        product.QuantityPerUnit = "10 boxes x 20 bags";
        product.UnitPrice = 18;
        product.UnitsInStock = 39;
        return product;
    }
}