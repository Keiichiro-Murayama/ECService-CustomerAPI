using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;

namespace ECService_CustomerAPI.Application.Usecases.Imps;

/// <summary>
/// 顧客アカウント情報取得Usecaseの実装
/// </summary>
public class GetCustomerInfoUsecase : IGetCustomerInfoUsecase
{
    private readonly ICustomerRepository _customerRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="customerRepository">顧客Repository</param>
    public GetCustomerInfoUsecase(
        ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    /// <summary>
    /// 顧客UUIDに紐づく顧客アカウント情報を取得する
    /// </summary>
    /// <param name="customerUuid">顧客UUID</param>
    /// <returns>顧客アカウント情報。存在しない場合はnull</returns>
    public async Task<Customer?> ExecuteAsync(
        string customerUuid)
    {
        return await _customerRepository.FindByUuidAsync(
            customerUuid);
    }
}