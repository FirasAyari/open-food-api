using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using open_food_api.DataAccess;
using open_food_api.Infrastructure.Services.Interfaces;

namespace open_food_api.Infrastructure.Services.Implementations
{
    public class ApiInfoService : IApiInfoService
    {
        private readonly DateTime _apiStartTime;
        private readonly Stopwatch _uptimeStopwatch;
        private readonly MongoDbContext _mongoDbContext;
        private readonly IConfiguration _configuration;
        public ApiInfoService(MongoDbContext mongoDbContext, IConfiguration configuration)
        {
            _apiStartTime = DateTime.UtcNow;
            _uptimeStopwatch = Stopwatch.StartNew();
            _mongoDbContext = mongoDbContext;
            _configuration = configuration;

        }

        public ApiDetails GetApiDetails()
        {
            bool isDatabaseConnectionOk = CheckDatabaseConnection();
            DateTime lastCronExecutionTime = GetLastCronExecutionTime();
            TimeSpan uptime = _uptimeStopwatch.Elapsed;
            float memoryUsage = GetMemoryUsage();

            var apiDetails = new ApiDetails
            {
                DatabaseConnection = isDatabaseConnectionOk,
                LastCronExecutionTime = lastCronExecutionTime,
                Uptime = uptime,
                MemoryUsage = memoryUsage
            };

            return apiDetails;
        }

        private bool CheckDatabaseConnection()
        {
            bool isDatabaseConnectionOk;

            try
            {
                // Attempt to execute a database operation to check the connection
                var products = _mongoDbContext.Products.Find(_ => true).ToList();
                isDatabaseConnectionOk = true;
            }
            catch (Exception)
            {
                isDatabaseConnectionOk = false;
            }

            return isDatabaseConnectionOk;
        }

        private DateTime GetLastCronExecutionTime()
        {
            string lastCronExecutionTimeString = _configuration["LastCronExecutionTime"];
            if (!DateTime.TryParse(lastCronExecutionTimeString, out DateTime lastCronExecutionTime))
            {
                // Default value if parsing fails or the configuration is not present
                lastCronExecutionTime = DateTime.MinValue;
            }

            return lastCronExecutionTime;
        }

        private float GetMemoryUsage()
        {
            Process currentProcess = Process.GetCurrentProcess();
            long memoryUsedBytes = currentProcess.WorkingSet64;
            float memoryUsage = memoryUsedBytes / (1024f * 1024f); // Convert to megabytes

            return memoryUsage;
        }
    }
}
