using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Presentation.ViewModels;

namespace ECService_CustomerAPI.Presentation.Adapters;

/// <summary>
/// 顧客ドメインを顧客アカウント情報レスポンスへ変換するAdapter
/// </summary>
public class GetCustomerInfoViewModelAdapter
{
    /// <summary>
    /// 顧客ドメインを顧客アカウント情報レスポンスへ変換する
    /// </summary>
    /// <param name="customer">顧客ドメイン</param>
    /// <returns>顧客アカウント情報レスポンス</returns>
    public CustomerInfoResponse ConvertAsync(Customer customer)
    {
        return new CustomerInfoResponse
        {
            CustomerUuid = customer.CustomerUuid,
            Name = customer.Name,
            NameKana = customer.NameKana,
            Address1 = customer.Address1,
            Address2 = customer.Address2,
            PhoneNumber = customer.PhoneNumber,
            MailAddress = customer.MailAddress,
            Username = customer.Username
        };
    }
}