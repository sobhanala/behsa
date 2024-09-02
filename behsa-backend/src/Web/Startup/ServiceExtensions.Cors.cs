using Microsoft.Extensions.DependencyInjection;

namespace Web.Startup
{
    public static partial class ServiceExtensions
    {
        public static void AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", corsPolicyBuilder =>
                {
                    corsPolicyBuilder.WithOrigins(
                        "http://localhost:4200",
                        "http://external.abriment.com:30081"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }
    }
}