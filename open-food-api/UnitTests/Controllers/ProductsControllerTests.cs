using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Nest;
using open_food_api.Controllers;
using open_food_api.Domain.Entities.Products;
using open_food_api.Infrastructure.Services.DTOs;
using open_food_api.Infrastructure.Services.Exceptions;
using open_food_api.Infrastructure.Services.Implementations;
using open_food_api.Infrastructure.Services.Interfaces;
using Xunit;

namespace open_food_api.UnitTests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly Mock<IApiInfoService> _mockApiInfoService;
        private readonly Mock<IElasticClient> _mockElasticClient;
        private readonly Mock<IApiKeyService> _mockApiKeyService;
        private readonly ProductsController _productsController;
        private string apiKey = "2e14258e8b24419281290b7bffc9308c";
        public ProductsControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _mockApiInfoService = new Mock<IApiInfoService>();
            _mockElasticClient = new Mock<IElasticClient>();
            _mockApiKeyService = new Mock<IApiKeyService>();
            _productsController = new ProductsController(
                _mockProductService.Object,
                _mockApiInfoService.Object,
                _mockElasticClient.Object,
                _mockApiKeyService.Object
            );

        }
        [Fact]
        public async Task UpdateProduct_ExistingProduct_ReturnsNoContent()
        {
            // Arrange
            int code = 1;
            var updatedProduct = new Product(
                code,
                ProductStatus.Published,
                DateTime.Now,
                "URL",
                "Creator",
                123456,
                123456,
                "Updated Product Name",
                "Quantity",
                "Brands",
                "Categories",
                "Labels",
                "Cities",
                "Purchase Places",
                "Stores",
                "Ingredients Text",
                "Traces",
                "Serving Size",
                1.5m,
                123,
                "Nutri Score Grade",
                "Main Category",
                "Image URL"
            );
          
            

            _mockApiKeyService.Setup(a => a.ValidateApiKey(apiKey)).Returns(true);
            _mockProductService.Setup(s => s.UpdateProduct(code, updatedProduct)).Verifiable();
            _mockElasticClient.Setup(c => c.UpdateAsync<Product>(
                code.ToString(),
                It.IsAny<Func<UpdateDescriptor<Product, Product>, IUpdateRequest<Product, Product>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UpdateResponse<Product>());

            // Act
            var result = await _productsController.UpdateProduct(code, updatedProduct, apiKey);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);

            // Verify the interactions
            _mockApiKeyService.VerifyAll();
            _mockProductService.VerifyAll();
            _mockElasticClient.VerifyAll();
        }


        [Fact]
        public async Task GetProduct_InvalidCode_ReturnsNotFound()
        {
            // Arrange
            int code = 20;
            _mockApiKeyService.Setup(a => a.ValidateApiKey(apiKey)).Returns(true);
            _mockProductService.Setup(s => s.GetProduct(code)).ReturnsAsync((Product)null);

            // Act
            var result = await _productsController.GetProduct(code, apiKey);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateProduct_ValidCode_ReturnsNoContent()
        {
            // Arrange
            int code = 1;
            var product = new Product(
                 code,
                 ProductStatus.Published,
                 DateTime.Now,
                 "URL",
                 "Creator",
                 123456,
                 123456,
                 "Updated Product Name",
                 "Quantity",
                 "Brands",
                 "Categories",
                 "Labels",
                 "Cities",
                 "Purchase Places",
                 "Stores",
                 "Ingredients Text",
                 "Traces",
                 "Serving Size",
                 1.5m,
                 123,
                 "Nutri Score Grade",
                 "Main Category",
                 "Image URL"
             );
            _mockApiKeyService.Setup(a => a.ValidateApiKey(apiKey)).Returns(true);
            _mockProductService.Setup(s => s.UpdateProduct(code, product)).Returns(Task.CompletedTask);
            _mockElasticClient.Setup(c => c.UpdateAsync<Product>(
                code.ToString(),
                It.IsAny<Func<UpdateDescriptor<Product, Product>, IUpdateRequest<Product, Product>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UpdateResponse<Product>());           
            // Act
            var result = await _productsController.UpdateProduct(code, product, apiKey);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateProduct_InvalidCode_ReturnsNotFound()
        {
            // Arrange
            int code = 1;
            var product = new Product(
                 code,
                 ProductStatus.Published,
                 DateTime.Now,
                 "URL",
                 "Creator",
                 123456,
                 123456,
                 "Updated Product Name",
                 "Quantity",
                 "Brands",
                 "Categories",
                 "Labels",
                 "Cities",
                 "Purchase Places",
                 "Stores",
                 "Ingredients Text",
                 "Traces",
                 "Serving Size",
                 1.5m,
                 123,
                 "Nutri Score Grade",
                 "Main Category",
                 "Image URL"
             );
            _mockApiKeyService.Setup(a => a.ValidateApiKey(apiKey)).Returns(true);
            _mockProductService.Setup(s => s.UpdateProduct(code, product)).Throws<ProductNotFoundException>();           
            // Act
            var result = await _productsController.UpdateProduct(code, product,apiKey);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetProducts_ReturnsListOfProducts()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;

            _mockApiKeyService.Setup(a => a.ValidateApiKey(apiKey)).Returns(true);
            var mockSearchResponse = new Mock<ISearchResponse<Product>>();
            mockSearchResponse.SetupGet(r => r.IsValid).Returns(true);
            mockSearchResponse.SetupGet(r => r.Documents).Returns(new List<Product>());

            _mockElasticClient.Setup(c => c.SearchAsync<Product>(It.IsAny<Func<SearchDescriptor<Product>, ISearchRequest>>(), default))
                .ReturnsAsync(mockSearchResponse.Object);

            // Act
            var result = await _productsController.GetProducts(pageNumber, pageSize, apiKey);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsType<List<Product>>(okObjectResult.Value);
            Assert.Empty(products);

            // Verify the interaction
            _mockElasticClient.Verify(c => c.SearchAsync<Product>(It.IsAny<Func<SearchDescriptor<Product>, ISearchRequest>>(), default), Times.Once);
        }
    }
}
