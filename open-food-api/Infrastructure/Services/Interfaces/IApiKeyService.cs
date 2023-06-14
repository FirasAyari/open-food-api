namespace open_food_api.Infrastructure.Services.Implementations
{
    public interface IApiKeyService
    {
        string GenerateApiKey();
        bool ValidateApiKey(string apiKey);
    }

}
