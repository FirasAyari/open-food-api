using open_food_api.Infrastructure.Services.Implementations;


var builder = WebApplication.CreateBuilder(args);

// Build the configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

// Call the ConfigureHangfireService method from HangfireService to configure Hangfire and set up the necessary services
HangfireService.ConfigureHangfireService(builder, configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
