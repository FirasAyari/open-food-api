using AutoMapper;
using open_food_api.Domain.Entities.Products;
using open_food_api.Infrastructure.Services.DTOs;

namespace open_food_api.Infrastructure.Services.Mappers
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
        }
    }
}
