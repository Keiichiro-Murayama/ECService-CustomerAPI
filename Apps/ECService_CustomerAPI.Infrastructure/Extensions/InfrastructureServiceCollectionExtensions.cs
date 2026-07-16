using ECService_CustomerAPI.Application.UnitOfWorks;
using ECService_CustomerAPI.Domain.Adapters;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;
using ECService_CustomerAPI.Infrastructure.Adapters;
using ECService_CustomerAPI.Infrastructure.Contexts;
using ECService_CustomerAPI.Infrastructure.Entities;
using ECService_CustomerAPI.Infrastructure.Repositories;
using ECService_CustomerAPI.Infrastructure.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ECService_CustomerAPI.Infrastructure.Extensions;

/// <summary>
/// インフラストラクチャ層の構成要素をDIコンテナへ登録する拡張メソッドを提供する。
/// </summary>
public static class InfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// インフラストラクチャ層の構成要素を登録する。
    /// </summary>
    /// <param name="services">DIコンテナ。</param>
    /// <param name="connectionString">接続文字列。</param>
    /// <returns>DIコンテナ。</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        // DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Adapter
        services.AddScoped<CustomerEntityAdapter>();
        services.AddScoped<OrdersEntityAdapter>();
        services.AddScoped<OrderDetailEntityAdapter>();
        services.AddScoped<PaymentMethodEntityAdapter>();

        // Adapter Interface
        services.AddScoped<
             IConverter<Customer, CustomerEntity>,
             CustomerEntityAdapter>();
        services.AddScoped<
             IConverter<Orders, OrdersEntity>,
             OrdersEntityAdapter>();
        services.AddScoped<
             IConverter<OrderDetail, OrderDetailEntity>,
             OrderDetailEntityAdapter>();
        services.AddScoped<
             IConverter<PaymentMethod, PaymentMethodEntity>,
             PaymentMethodEntityAdapter>();

        services.AddScoped<
             IRestorer<Customer, CustomerEntity>,
             CustomerEntityAdapter>();
        services.AddScoped<
             IRestorer<Orders, OrdersEntity>,
             OrdersEntityAdapter>();
        services.AddScoped<
             IRestorer<OrderDetail, OrderDetailEntity>,
             OrderDetailEntityAdapter>();
        services.AddScoped<
             IRestorer<PaymentMethod, PaymentMethodEntity>,
             PaymentMethodEntityAdapter>();

        // UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repository
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrdersRepository, OrdersRepository>();
        services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();

        return services;
    }
}