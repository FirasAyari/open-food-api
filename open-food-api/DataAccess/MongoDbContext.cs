using MongoDB.Bson;
using MongoDB.Driver;
using open_food_api.Domain.Entities.Products;
using System;

namespace open_food_api.DataAccess
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        public IMongoCollection<Product> Products { get; }

        public MongoDbContext()
        {
            const string connectionString = "mongodb://localhost:27017/"; // Update the connection string

            var settings = MongoClientSettings.FromConnectionString(connectionString);

            // Create a new client and connect to the server
            var client = new MongoClient(settings);

            // Get the database
            _database = client.GetDatabase("openfoodfacts"); // Replace "YourDatabaseName" with your actual database name

            // Get the collection
            Products = _database.GetCollection<Product>("foodProducts");
            Console.WriteLine(Products);

            // Send a ping to confirm a successful connection
            try
            {
                var result = _database.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                Console.WriteLine(result);
                Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
