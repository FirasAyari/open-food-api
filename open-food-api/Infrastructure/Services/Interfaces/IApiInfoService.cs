namespace open_food_api.Infrastructure.Services.Interfaces
{
    public interface IApiInfoService
    {
        ApiDetails GetApiDetails();
    }

    public class ApiDetails
    {
        public bool DatabaseConnection { get; set; }
        public DateTime LastCronExecutionTime { get; set; }
        public TimeSpan Uptime { get; set; }
        public float MemoryUsage { get; set; }
    }
}
