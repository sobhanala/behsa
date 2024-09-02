# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything and restore as distinct layers
COPY . ./
RUN dotnet restore

# Build and publish the app to the out directory
RUN dotnet publish src/Web/Web.csproj -c Release -o out

# Use the official ASP.NET image to serve the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Environment variables for the database connection
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ConnectionStrings__DefaultConnection=Host=db;Database=YourDatabaseName;Username=yourusername;Password=yourpassword
ENV ASPNETCORE_URLS=http://*:80
EXPOSE 80


# Start the application
ENTRYPOINT ["dotnet", "Web.dll"]



