using Web.Startup;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
builder.Services.AddInfrastructureServices(config);
builder.Services.ConfigureAppAuthenticationServices(config);
builder.Services.AddApplicationServices();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddCorsPolicy();


var app = builder.Build();
app.UseMiddlewareServices();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services, config);
}
app.Run();