using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration;
using Hangfire.Mongo;
using Hangfire;
using MongoDB.Driver;
using open_food_api.DataAccess.Interfaces;
using open_food_api.DataAccess;
using open_food_api.Infrastructure.Services.Interfaces;
using open_food_api.DataAccess.Repositories;
using Nest;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using open_food_api.Domain.Entities.Products;
using Microsoft.OpenApi.Models;


namespace open_food_api.Infrastructure.Services.Implementations
{
    public class HangfireService
    {
        public static void ConfigureHangfireService(WebApplicationBuilder builder, IConfiguration configuration)
        {        
            // Configure logging with Elasticsearch sink
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = "hangfire-logs",
                })
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                Log.Information("Starting up...");

                builder.Services.AddControllers()
                    .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    });                               

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "1.0",
                        Title = "API Documentation",
                        Description = "Documentation for your API",
                        Contact = new OpenApiContact
                        {
                            Name = "Firas",
                            Email = "firas.ayari@esprit.tn",
                            Url = new Uri("https://example.com"),
                        },
                    });
                });

                builder.Services.AddScoped<IApiInfoService, ApiInfoService>();
                builder.Services.AddScoped<IProductRepository, ProductRepository>();
                


                // Register MongoDbContext as a service
                builder.Services.AddSingleton<MongoDbContext>();
                builder.Services.AddScoped<IProductService, ProductService>();
                builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

                // Register IMongoDatabase as a service
                var connectionString = "mongodb://localhost:27017";
                var databaseName = "openfoodfacts";
                var mongoClient = new MongoClient(connectionString);
                var mongoDatabase = mongoClient.GetDatabase(databaseName);
                builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);

                var elasticUri = new Uri("http://localhost:9200/");
                var connectionSettings = new ConnectionSettings(elasticUri)
                    .BasicAuthentication("admin", "123456");
                var elasticClient = new ElasticClient(connectionSettings);

                var indexName = "products";

                // Create the index if it doesn't exist
                var createIndexResponse = elasticClient.Indices.Create(indexName, c => c
                    .Map<Product>(m => m.AutoMap())
                );

                if (!createIndexResponse.IsValid)
                {
                    // Handle index creation failure
                    Console.WriteLine($"Failed to create index: {createIndexResponse.ServerError.Error}");
                }

                builder.Services.AddSingleton<IElasticClient>(elasticClient);

                // Configure Hangfire with MongoDB storage
                builder.Services.AddHangfire(config =>
                {
                    config.UseMongoStorage(connectionString, databaseName, new MongoStorageOptions
                    {
                        MigrationOptions = new MongoMigrationOptions
                        {
                            MigrationStrategy = new MigrateMongoMigrationStrategy()
                        },
                        MigrationLockTimeout = TimeSpan.FromMinutes(5)
                    });
                });

                var app = builder.Build();

                // Initialize Hangfire
                InitializeHangFire(app, connectionString, databaseName, elasticClient);

                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void InitializeHangFire(WebApplication app, string connectionString, string databaseName, ElasticClient elasticClient)
        {
            // Perform migration if necessary
            var mongoClient = new MongoClient(connectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseName);
            var migrationOptions = new MongoStorageOptions
            {
                MigrationOptions = new MongoMigrationOptions
                {
                    MigrationStrategy = new MigrateMongoMigrationStrategy()
                }
            };
            var migrationManager = new MongoMigrationManager(migrationOptions, mongoDatabase);
            MongoMigrationManager.MigrateIfNeeded(migrationOptions, mongoDatabase);

            // Schedule the DataImportJob to run daily
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var productDatabase = serviceProvider.GetRequiredService<IMongoDatabase>();
                var recurringJobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();

                var dataImportJob = new DataImportJob(productDatabase, recurringJobManager, elasticClient);
                dataImportJob.ScheduleImportJob();
            }

            // Use Hangfire server and dashboard
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            // Use Swagger and Swagger UI
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Documentation");
            });
        }
    }
}
