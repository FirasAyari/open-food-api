using open_food_api.Domain.Entities;
using open_food_api.Domain.Entities.Products;

namespace open_food_api.Infrastructure.Services.Interfaces
{
    public interface IProductService
    {
        Task<Product> GetProduct(int code);
        Task<List<Product>> GetProducts(PaginationParameters paginationParameters);
        Task UpdateProduct(int code, Product product);        
        Task ChangeProductStatus(int code, string status);

    }
}
