using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using open_food_api.Infrastructure.Services.Implementations;

namespace open_food_api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class ApiKeyController : ControllerBase
    {
        private readonly IApiKeyService _apiKeyService;

        public ApiKeyController(IApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService;
        }

        [HttpGet]
        public IActionResult GetApiKey()
        {
            var apiKey = _apiKeyService.GenerateApiKey();
            return Ok(new { ApiKey = apiKey });
        }
    }
}
