using Microsoft.EntityFrameworkCore;
using OMSBlazor.Dto.Order;
using OMSBlazor.Interfaces.ApplicationServices;
using OMSBlazor.Northwind.OrderAggregate;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace OMSBlazor.ApplicationServices;

public class OrderApplicationServiceTests : OMSBlazorApplicationTestBase
{
    private readonly IOrderApplicationService _orderApplicationService;
    private readonly IRepository<Order, int> _orderRepository;
    private readonly IRepository<Product, int> _productRepository;
    private readonly IRepository<Category, int> _categoryRepository;
    private readonly IRepository<Employee, int> _employeeRepository;
    private readonly IRepository<Customer, string> _customerRepository;

    public OrderApplicationServiceTests()
    {
        _orderApplicationService = GetRequiredService<IOrderApplicationService>();
        _orderRepository = GetRequiredService<IRepository<Order, int>>();
        _productRepository = GetRequiredService<IRepository<Product, int>>();
        _categoryRepository = GetRequiredService<IRepository<Category, int>>();
        _employeeRepository = GetRequiredService<IRepository<Employee, int>>();
        _customerRepository = GetRequiredService<IRepository<Customer, string>>();
    }

    [Fact]
    public async Task SaveOrderAsync_Should_Persist_Order_With_Details()
    {
        const int categoryId = 9999;
        const int productId = 10001;
        const int employeeId = 20001;
        const string customerId = "ZZZZZ";

        await WithUnitOfWorkAsync(async () =>
        {
            var category = CreateCategory(categoryId, "Beverages");
            await _categoryRepository.InsertAsync(category);

            var product = CreateProduct(productId, "Chai", category.Id);
            await _productRepository.InsertAsync(product, autoSave: true);

            var employee = new Employee(employeeId, "Nancy", "Davolio");
            await _employeeRepository.InsertAsync(employee, autoSave: true);

            var customer = CreateCustomer(customerId, "Alfreds Futterkiste");
            await _customerRepository.InsertAsync(customer, autoSave: true);
        });

        var createOrderDto = new CreateOrderDto
        {
            EmployeeId = employeeId,
            CustomerId = customerId,
            RequiredDate = DateTime.Today,
            ShipRegion = "WA",
            ShipName = "Speedy Express",
            ShipCountry = "USA",
            ShipCity = "Seattle",
            ShipAddress = "1st Avenue",
            ShipPostalCode = "98052",
            OrderDetails = new List<OrderDetailDto>
            {
                new() { ProductId = productId, Quantity = 2, UnitPrice = 10.5, Discount = 0 }
            }
        };

        var orderDto = await _orderApplicationService.SaveOrderAsync(createOrderDto);

        orderDto.ShipCity.ShouldBe("Seattle");
        orderDto.OrderDetails.Count.ShouldBe(1);

        var storedOrder = await WithUnitOfWorkAsync(async () =>
        {
            var q = (await _orderRepository.GetQueryableAsync()).Include(x=>x.OrderDetails);
            return await q.SingleAsync(o => o.Id == orderDto.OrderId);
        });
        storedOrder.OrderDetails.Count.ShouldBe(1);
        storedOrder.OrderDetails[0].Quantity.ShouldBe(2);
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

    private static Customer CreateCustomer(string id, string companyName)
    {
        var customer = (Customer)Activator.CreateInstance(typeof(Customer), true)!;
        typeof(Customer).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(customer, id);
        customer.SetCompanyName(companyName);
        return customer;
    }
}