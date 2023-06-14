using Newtonsoft.Json;


namespace open_food_api.Infrastructure.Services.Implementations
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IDictionary<string, bool> _apiKeys;
        private const string ApiKeysFilePath = "C:\\Users\\firas\\source\\repos\\open-food-api\\open-food-api\\apiKeys.json";

        public ApiKeyService()
        {
            _apiKeys = new Dictionary<string, bool>();
            LoadApiKeys();
        }

        public string GenerateApiKey()
        {
            string apiKey = Guid.NewGuid().ToString("N");
            _apiKeys.Add(apiKey, true);
            SaveApiKeys();
            return apiKey;
        }

        public bool ValidateApiKey(string apiKey)
        {
            if (apiKey == null)
            {
                return false;
            }

            return IsApiKeyValid(apiKey);
        }

        private bool IsApiKeyValid(string apiKey)
        {
            if (File.Exists(ApiKeysFilePath))
            {
                string json = File.ReadAllText(ApiKeysFilePath);
                var storedApiKeys = JsonConvert.DeserializeObject<Dictionary<string, bool>>(json);
                return storedApiKeys.ContainsKey(apiKey);
            }

            return false;
        }

        private IDictionary<string, bool> LoadApiKeys()
        {
            if (File.Exists(ApiKeysFilePath))
            {
                string json = File.ReadAllText(ApiKeysFilePath);
                return JsonConvert.DeserializeObject<Dictionary<string, bool>>(json);
            }

            return new Dictionary<string, bool>();
        }

        private void SaveApiKeys()
        {
            string json = JsonConvert.SerializeObject(_apiKeys);
            File.WriteAllText(ApiKeysFilePath, json);
        }
    }
}
