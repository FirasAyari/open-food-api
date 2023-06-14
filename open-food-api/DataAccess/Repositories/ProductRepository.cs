using MongoDB.Driver;
using MongoDB.Driver.Linq;
using open_food_api.DataAccess.Interfaces;
using open_food_api.Domain.Entities;
using open_food_api.Domain.Entities.Products;
using open_food_api.Infrastructure.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace open_food_api.DataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _collection;

        public ProductRepository(MongoDbContext dbContext)
        {
            _collection = dbContext.Products;
        }

        public async Task<Product> GetByCodeAsync(int code, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Code, code);
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _collection.Find(_ => true).ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(product, cancellationToken: cancellationToken);
        }

        public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Code, product.Code);
            await _collection.ReplaceOneAsync(filter, product, cancellationToken: cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Code, id);
            await _collection.DeleteOneAsync(filter, cancellationToken);
        }

        public async Task<PaginationResult<Product>> GetProductsAsync(PaginationParameters paginationParameters)
        {
            var query = _collection.AsQueryable();

            // Calculate the skip count based on the page and page size
            var skipCount = (paginationParameters.PageNumber - 1) * paginationParameters.PageSize;

            // Retrieve the total count of products without pagination            
            var totalCount = await query.CountAsync(x => true);


            // Apply pagination to the query
            var products = await query.Skip(skipCount).Take(paginationParameters.PageSize).ToListAsync();

            return new PaginationResult<Product>(products, totalCount);
        }
    }
}
