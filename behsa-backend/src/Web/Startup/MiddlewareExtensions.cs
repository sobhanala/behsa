namespace Web.Startup;

public static class MiddlewareExtensions
{
    public static WebApplication UseMiddlewareServices(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowSpecificOrigins");
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();

        return app;
    }
}
