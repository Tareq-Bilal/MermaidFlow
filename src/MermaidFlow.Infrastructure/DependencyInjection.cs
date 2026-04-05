using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Infrastructure.Common.Persistence;
using MermaidFlow.Infrastructure.Common.Security;
using MermaidFlow.Infrastructure.Documents;
using MermaidFlow.Infrastructure.Persistence;
using MermaidFlow.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MermaidFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MermaidFlowDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IDocumentsRepository, DocumentsRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
