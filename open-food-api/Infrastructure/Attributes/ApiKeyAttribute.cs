namespace open_food_api.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAttribute : Attribute
    {
        public string ApiKey { get; }

        public ApiKeyAttribute(string apiKey)
        {
            ApiKey = apiKey;
        }
    }
}
