using Hangfire;
using Hangfire.Common;
using Hangfire.Storage;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json;
using open_food_api.Domain.Entities.Products;
using System;
using System.IO;
using System.Net;
using Nest;

public class DataImportJob
{
    private readonly IMongoDatabase _database;
    private readonly string _openFoodFactsBaseUrl;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IElasticClient _elasticClient;

    public DataImportJob(IMongoDatabase database, IRecurringJobManager recurringJobManager, IElasticClient elasticClient)
    {
        _database = database;
        _openFoodFactsBaseUrl = "https://challenges.coode.sh/food/data/json/";
        _recurringJobManager = recurringJobManager;
        _elasticClient = elasticClient;
    }

    public void ImportData()
    {
        // Get the list of available files from the Open Food Facts API
        var filesUrl = _openFoodFactsBaseUrl + "index.txt";
        var fileNames = GetFileNames(filesUrl);

        // Import data for each file
        foreach (var fileName in fileNames)
        {
            // Download the file from the API
            var fileUrl = _openFoodFactsBaseUrl + fileName;
            var json = DownloadFile(fileUrl);

            // Deserialize the JSON data into an array of products
            var products = JsonConvert.DeserializeObject<Product[]>(json);

            // Insert or update the products in the MongoDB collection and Elasticsearch index
            var collection = _database.GetCollection<Product>("products");
            foreach (var product in products)
            {
                // Upsert the product based on its Code field in MongoDB
                var filter = Builders<Product>.Filter.Eq(p => p.Code, product.Code);
                collection.ReplaceOne(filter, product, new ReplaceOptions { IsUpsert = true });

                // Index the product in Elasticsearch
                _elasticClient.IndexDocument(product);
            }
        }
    }

    public void ScheduleImportJob()
    {
        _recurringJobManager.AddOrUpdate<DataImportJob>("import-data-job", job => job.ImportData(), Cron.Daily);
    }

    private string[] GetFileNames(string filesUrl)
    {
        using (var client = new WebClient())
        {
            var fileNames = client.DownloadString(filesUrl).Split(Environment.NewLine);
            return fileNames;
        }
    }

    private string DownloadFile(string fileUrl)
    {
        using (var client = new WebClient())
        {
            return client.DownloadString(fileUrl);
        }
    }
}
