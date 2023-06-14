namespace open_food_api.Infrastructure.Services.DTOs
{
    public class ProductDto
    {
        public int Code { get; set; }
        public ProductStatus Status { get; set; }
        public int ImportedTime { get; set; }
        public string Url { get; set; }
        public string Creator { get; set; }
        public int CreatedTime { get; set; }
        public int LastModifiedTime { get; set; }
        public string ProductName { get; set; }
        public string Quantity { get; set; }
        public string Brands { get; set; }
        public string Categories { get; set; }
        public string Labels { get; set; }
        public string Cities { get; set; }
        public string PurchasePlaces { get; set; }
        public string Stores { get; set; }
        public string IngredientsText { get; set; }
        public string Traces { get; set; }
        public string ServingSize { get; set; }
        public decimal ServingQuantity { get; set; }
        public int NutriScoreScore { get; set; }
        public string NutriScoreGrade { get; set; }
        public string MainCategory { get; set; }
        public string ImageUrl { get; set; }
    }

    public enum ProductStatus
    {        
        Draft,
        Published,
        Trash        
    }
}