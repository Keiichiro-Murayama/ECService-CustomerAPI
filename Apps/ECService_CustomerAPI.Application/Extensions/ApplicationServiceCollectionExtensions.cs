using ECService_CustomerAPI.Application.Usecases.Imps;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Application.Authentications;
using Microsoft.Extensions.DependencyInjection;

namespace ECService_CustomerAPI.Application.Extensions;

/// <summary>
/// アプリケーション層の構成要素をDIコンテナへ登録する拡張メソッドを提供する。
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
    /// <summary>
    /// アプリケーション層の構成要素を登録する。
    /// </summary>
    /// <param name="services">DIコンテナ。</param>
    /// <returns>DIコンテナ。</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ILoginUsecase, LoginUsecase>();
        services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
        services.AddScoped<IPurchaseUsecase, PurchaseUsecase>();
        services.AddScoped<IGetOrderHistoriesUsecase, GetOrderHistoriesUsecase>();
        services.AddScoped<IGetOrderDetailsUsecase, GetOrderDetailsUsecase>();
        services.AddScoped<IRegisterCustomerUsecase, RegisterCustomerUsecase>();
        services.AddScoped<IGetPaymentMethodsUsecase, GetPaymentMethodsUsecase>();
        services.AddScoped<IGetCustomerInfoUsecase, GetCustomerInfoUsecase>();

        return services;
    }
}