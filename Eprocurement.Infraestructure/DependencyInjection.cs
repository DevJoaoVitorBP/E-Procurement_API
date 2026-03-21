using Eprocurement.Application.Abstractions;
using Eprocurement.Domain.Interfaces;
using Eprocurement.Infrastructure.Identity;
using Eprocurement.Infrastructure.Persistence;
using Eprocurement.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eprocurement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ProcurementDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IPurchaseRequestRepository, PurchaseRequestRepository>();
            services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddScoped<IPurchaseHistoryRepository, PurchaseHistoryRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ProcurementDbContext>());

            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

            return services;
        }
    }
}