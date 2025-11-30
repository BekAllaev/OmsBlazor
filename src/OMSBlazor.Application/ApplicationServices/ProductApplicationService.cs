using OMSBlazor.Dto.Product;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Application.Services;
using OMSBlazor.Northwind.OrderAggregate;
using OMSBlazor.DomainManagers.Product;
using OMSBlazor.Dto.Product.Stastics;
using OMSBlazor.Northwind.Stastics;
using OMSBlazor.Interfaces.ApplicationServices;
using System.Linq;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System;

namespace OMSBlazor.Application.ApplicationServices
{
    public class ProductApplicationService : ApplicationService, IProductApplicationService
    {
        private readonly IRepository<Product, int> _productRepository;
        private readonly IReadOnlyRepository<Product, int> _productReadOnlyRepository; // AsNoTracking behind the scenes for Get requests 
        private readonly IRepository<ProductsByCategory, int> _productsByCategoryRepository;
        private readonly IProductManager _productManager;
        private readonly IDistributedCache<List<ProductDto>> _productCache; // added InMemory cache for products

        public ProductApplicationService(
            IRepository<Product, int> productRepository,
            IReadOnlyRepository<Product, int> productReadOnlyRepository,
            IProductManager productManager,
            IRepository<ProductsByCategory, int> productsByCategoryRepository,
            IDistributedCache<List<ProductDto>> productCache)
        {
            _productRepository = productRepository;
            _productManager = productManager;
            _productReadOnlyRepository = productReadOnlyRepository;
            _productsByCategoryRepository = productsByCategoryRepository;
            _productCache = productCache;
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            // InMemory cache for products
            var products = await _productCache.GetOrAddAsync(
                "products",
                async () => 
                {
                    var list = await _productReadOnlyRepository.ToListAsync();
                    return ObjectMapper.Map<List<Product>, List<ProductDto>>(list);
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = System.DateTimeOffset.Now.AddHours(1)
                }, 
                null, 
                false);

            if (products is null)
            {
                throw new Exception(OMSBlazorDomainErrorCodes.ProductListIsNull);
            }
            
            return products;
        }

        public async Task<ProductDto> GetProductAsync(int id)
        {
            var product = await _productRepository.GetAsync(id);

            var productDto = ObjectMapper.Map<Product, ProductDto>(product);

            return productDto;
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productManager.ThrowIfCannotDeleteAsync(id);

            await _productRepository.DeleteAsync(id);
        }

        public async Task CreateProductAsync(CreateProductDto productDto)
        {
            var product = await _productManager.CreateAsync(productDto.ProductName, productDto.CategoryId);
            product.SetProductName(productDto.ProductName);
            product.Discontinued = productDto.Discontinued;
            product.UnitsOnOrder = productDto.UnitsOnOrder;
            product.QuantityPerUnit = productDto.QuantityPerUnit;
            product.UnitPrice = productDto.UnitPrice;
            product.ReorderLevel = productDto.ReorderLevel;
            product.UnitsInStock = productDto.UnitsInStock;

            await _productRepository.InsertAsync(product);
        }

        public async Task UpdateProductAsync(int id, UpdateProductDto productDto)
        {
            var product = await _productManager.UpdateAsync(id, productDto.ProductName);
            product.Discontinued = productDto.Discontinued;
            product.UnitsOnOrder = productDto.UnitsOnOrder;
            product.QuantityPerUnit = productDto.QuantityPerUnit;
            product.UnitPrice = productDto.UnitPrice;
            product.ReorderLevel = productDto.ReorderLevel;
            product.UnitsInStock = productDto.UnitsInStock;

            await _productRepository.UpdateAsync(product);
        }

        public async Task<IEnumerable<ProductsByCategoryDto>> GetProductsByCategoryAsync()
        {
            var stastics = await _productsByCategoryRepository.GetListAsync();

            var stasticsDto = ObjectMapper.Map<List<ProductsByCategory>, List<ProductsByCategoryDto>>(stastics);

            return stasticsDto;
        }
    }
}