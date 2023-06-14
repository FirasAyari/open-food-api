using open_food_api.Domain.Entities;
using open_food_api.Domain.Entities.Products;
using open_food_api.Infrastructure.Services.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace open_food_api.DataAccess.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetByCodeAsync(int code, CancellationToken cancellationToken = default);
        Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Product product, CancellationToken cancellationToken = default);
        Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<PaginationResult<Product>> GetProductsAsync(PaginationParameters paginationParameters);
    }
}
