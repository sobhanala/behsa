using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services.DomainService;
using Application.Services.SharedService;
using Infrastructure.Repositories;
using Web.Services;

namespace Web.Startup;

public static partial class ServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IFileReaderService, CsvReaderService>();
        services.AddScoped<IRoleManagerRepository, RoleManagerRepository>();
        services.AddScoped<IUserManagerRepository, UserManagerRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IAccountService, AccountService>();
    }
}