using open_food_api.DataAccess.Interfaces;
using open_food_api.Domain.Entities;
using open_food_api.Domain.Entities.Products;
using open_food_api.Infrastructure.Services.DTOs;
using open_food_api.Infrastructure.Services.Exceptions;
using open_food_api.Infrastructure.Services.Interfaces;

namespace open_food_api.Infrastructure.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> GetProduct(int code)
        {
            try
            {
                return await _productRepository.GetByCodeAsync(code);
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging
                Console.WriteLine(ex.Message);
                throw; // Rethrow the exception to be handled at a higher level
            }
        }
        public async Task<Product> GetProductByCodeAsync(int code)
        {
            return await _productRepository.GetByCodeAsync(code);
        }     

        public async Task UpdateProduct(int code, Product product)
        {
            var productToUpdate = await _productRepository.GetByCodeAsync(code);
            if (productToUpdate != null)
            {
                // Update the product properties based on the provided product object                
                productToUpdate.Status = product.Status;
                productToUpdate.ImportedTime = product.ImportedTime;
                productToUpdate.Url = product.Url;
                productToUpdate.Creator = product.Creator;
                productToUpdate.CreatedTime = product.CreatedTime;
                productToUpdate.LastModifiedTime = product.LastModifiedTime;
                productToUpdate.ProductName = product.ProductName;
                productToUpdate.Quantity = product.Quantity;
                productToUpdate.Brands = product.Brands;
                productToUpdate.Categories = product.Categories;
                productToUpdate.Labels = product.Labels;
                productToUpdate.Cities = product.Cities;
                productToUpdate.PurchasePlaces = product.PurchasePlaces;
                productToUpdate.Stores = product.Stores;
                productToUpdate.IngredientsText = product.IngredientsText;
                productToUpdate.Traces = product.Traces;
                productToUpdate.ServingSize = product.ServingSize;
                productToUpdate.ServingQuantity = product.ServingQuantity;
                productToUpdate.NutriScoreScore = product.NutriScoreScore;
                productToUpdate.NutriScoreGrade = product.NutriScoreGrade;
                productToUpdate.MainCategory = product.MainCategory;
                productToUpdate.ImageUrl = product.ImageUrl;

                await _productRepository.UpdateAsync(productToUpdate);
            }
        }

        public async Task ChangeProductStatus(int code, string status)
        {
            // Retrieve the product from the repository using the code
            var product = await _productRepository.GetByCodeAsync(code);
            if (product == null)
            {
                throw new ProductNotFoundException(code);
            }

            // Convert the string status to ProductStatus enum
            if (Enum.TryParse(status, true, out ProductStatus productStatus))
            {
                // Update the product status
                product.Status = productStatus;

                // Save the changes to the repository
                await _productRepository.UpdateAsync(product);
            }
            else
            {
                throw new InvalidProductStatusException(status);
            }
        }

        public async Task<List<Product>> GetProducts(PaginationParameters paginationParameters)
        {
            var paginationResult = await _productRepository.GetProductsAsync(paginationParameters);
            return paginationResult.Items;
        }
    }
}