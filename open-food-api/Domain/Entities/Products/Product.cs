using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using open_food_api.Infrastructure.Services.DTOs;

namespace open_food_api.Domain.Entities.Products
{
    [ElasticsearchType(IdProperty = nameof(Id))]
    public class Product
    {
        private readonly List<string> _validStatuses = new List<string> { "draft", "published", "trash" };

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string Id { get; set; }

        [Number(NumberType.Integer, Name = "code")]
        [BsonElement("code")]
        public int Code { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonElement("status")]
        public ProductStatus Status { get; set; }

        [BsonElement("imported_t")]
        public DateTime ImportedTime { get; set; }

        [BsonElement("url")]
        public string Url { get; set; }

        [BsonElement("creator")]
        public string Creator { get; set; }

        [BsonElement("created_t")]
        public int CreatedTime { get; set; }

        [BsonElement("last_modified_t")]
        public int LastModifiedTime { get; set; }

        [BsonElement("product_name")]
        public string ProductName { get; set; }


        [BsonElement("quantity")]
        public string Quantity { get; set; }

        [BsonElement("brands")]
        public string Brands { get; set; }

        [BsonElement("categories")]
        public string Categories { get; set; }

        [BsonElement("labels")]
        public string Labels { get; set; }

        [BsonElement("cities")]
        public string Cities { get; set; }

        [BsonElement("purchase_places")]
        public string PurchasePlaces { get; set; }

        [BsonElement("stores")]
        public string Stores { get; set; }

        [BsonElement("ingredients_text")]
        public string IngredientsText { get; set; }

        [BsonElement("traces")]
        public string Traces { get; set; }

        [BsonElement("serving_size")]
        public string ServingSize { get; set; }

        [BsonElement("serving_quantity")]
        public decimal ServingQuantity { get; set; }

        [BsonElement("nutriscore_score")]
        public int NutriScoreScore { get; set; }

        [BsonElement("nutriscore_grade")]
        public string NutriScoreGrade { get; set; }

        [BsonElement("main_category")]
        public string MainCategory { get; set; }

        [BsonElement("image_url")]
        public string ImageUrl { get; set; }

        [Text(Name = "search_field")]
        [BsonIgnore]
        public string SearchField => $"{ProductName} {Quantity} {Brands} {Categories} {Labels} {Cities} {PurchasePlaces} {Stores} {IngredientsText} {Traces} {ServingSize}";

        public Product(int code, ProductStatus status, DateTime importedTime, string url, string creator, int createdTime, int lastModifiedTime,
                       string productName, string quantity, string brands, string categories, string labels, string cities,
                       string purchasePlaces, string stores, string ingredientsText, string traces, string servingSize,
                       decimal servingQuantity, int nutriScoreScore, string nutriScoreGrade, string mainCategory, string imageUrl)
        {
            ValidateStatus(status);

            Code = code;
            Status = status;
            ImportedTime = importedTime;
            Url = url;
            Creator = creator;
            CreatedTime = createdTime;
            LastModifiedTime = lastModifiedTime;
            ProductName = productName;
            Quantity = quantity;
            Brands = brands;
            Categories = categories;
            Labels = labels;
            Cities = cities;
            PurchasePlaces = purchasePlaces;
            Stores = stores;
            IngredientsText = ingredientsText;
            Traces = traces;
            ServingSize = servingSize;
            ServingQuantity = servingQuantity;
            NutriScoreScore = nutriScoreScore;
            NutriScoreGrade = nutriScoreGrade;
            MainCategory = mainCategory;
            ImageUrl = imageUrl;
        }

        private void ValidateStatus(ProductStatus status)
        {
            if (!_validStatuses.Contains(status.ToString().ToLower()))
            {
                throw new ArgumentException("Invalid status value.");
            }
        }
    }
}
