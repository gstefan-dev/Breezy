using Azure.Storage.Blobs;
using Breezy.Application.Interfaces;
using Breezy.Infrastructure.Persistence;
using Breezy.Infrastructure.Persistence.Repositories;
using Breezy.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Breezy.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<BreezyDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<ISoundPackRepository, SoundPackRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        
        services.AddSingleton(new BlobServiceClient(
            configuration["AzureBlobStorage:ConnectionString"]));

        services.AddScoped<IAzureBlobService, AzureBlobService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}