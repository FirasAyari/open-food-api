using Microsoft.AspNetCore.Mvc;
using Nest;
using open_food_api.Domain.Entities.Products;
using open_food_api.Infrastructure.Attributes;
using open_food_api.Infrastructure.Services.Exceptions;
using open_food_api.Infrastructure.Services.Implementations;
using open_food_api.Infrastructure.Services.Interfaces;


namespace open_food_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiKey("API_KEY")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IApiInfoService _apiInfoService;
        private readonly IElasticClient _elasticClient;
        private readonly string _defaultIndex;
        private readonly IApiKeyService _apiKeyService;

        public ProductsController(IProductService productService, IApiInfoService apiInfoService, IElasticClient elasticClient, IApiKeyService apiKeyService)
        {
            _productService = productService;
            _apiInfoService = apiInfoService;
            _elasticClient = elasticClient;
            _defaultIndex = "products";
            _apiKeyService = apiKeyService;
        }

        [HttpGet("/")]
        [ProducesResponseType(typeof(ApiDetails), 200)]
        public IActionResult GetApiDetails()
        {
            var apiDetails = _apiInfoService.GetApiDetails();
            return Ok(apiDetails);
        }

        [HttpPut("products/{code}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProduct(int code, Product product, string apiKey)
        {
            if (!_apiKeyService.ValidateApiKey(apiKey))
            {
                return Unauthorized();
            }

            try
            {
                await _productService.UpdateProduct(code, product);

                // Update the product in Elasticsearch
                await _elasticClient.UpdateAsync<Product>(code.ToString(), u => u.Doc(product).Index(_defaultIndex));

                return NoContent();
            }
            catch (ProductNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpDelete("products/{code}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProduct(int code, string apiKey)
        {
            if (!_apiKeyService.ValidateApiKey(apiKey))
            {
                return Unauthorized();
            }

            try
            {
                await _productService.ChangeProductStatus(code, "trash");

                // Delete the product from Elasticsearch
                await _elasticClient.DeleteAsync<Product>(code.ToString(), d => d.Index(_defaultIndex));

                return NoContent();
            }
            catch (ProductNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("products/{code}")]
        [ApiKey("YOUR_API_KEY")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProduct(int code, string apiKey)
        {

            if (!_apiKeyService.ValidateApiKey(apiKey))
            {
                return Unauthorized();
            }

            try
            {
                var product = await _productService.GetProduct(code);
                if (product == null)
                {
                    return NotFound();
                }

                // Retrieve the product from Elasticsearch
                var elasticResponse = await _elasticClient.GetAsync<Product>(code.ToString(), g => g.Index(_defaultIndex));
                if (elasticResponse.IsValid)
                {
                    var elasticProduct = elasticResponse.Source;
                    product.MainCategory = elasticProduct.MainCategory; // Update the MainCategory field from Elasticsearch
                }

                return Ok(product);
            }
            catch (ProductNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("products")]
        [ProducesResponseType(typeof(List<Product>), 200)]
        public async Task<IActionResult> GetProducts([FromQuery] int pageNumber, [FromQuery] int pageSize, string apiKey)
        {
            if (!_apiKeyService.ValidateApiKey(apiKey))
            {
                return Unauthorized();
            }

            var searchResponse = await _elasticClient.SearchAsync<Product>(s => s
                .Index(_defaultIndex)
                .From(pageNumber)
                .Size(pageSize)
                .Sort(sort => sort.Descending(p => p.Id)));

            if (!searchResponse.IsValid)
            {
                // Handle search response error
                return StatusCode((int)searchResponse.ApiCall.HttpStatusCode, searchResponse.OriginalException);
            }

            var products = searchResponse.Documents;

            return Ok(products);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(List<Product>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> SearchProducts(string query, string apiKey)
        {

            if (!_apiKeyService.ValidateApiKey(apiKey))
            {
                return Unauthorized();
            }

            // Implement search logic using the Elasticsearch client
            var searchResponse = await _elasticClient.SearchAsync<Product>(s => s
                .Index(_defaultIndex)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.ProductName)
                        .Query(query)
                    )
                )
            );

            // Process search response and return the results
            if (searchResponse.IsValid)
            {
                var products = searchResponse.Documents.ToList();
                return Ok(products);
            }

            return BadRequest(searchResponse.DebugInformation);
        }
    }
}
