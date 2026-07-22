using ECService_CustomerAPI.Application.Authentications;
using ECService_CustomerAPI.Application.Exceptions;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;

namespace ECService_CustomerAPI.Application.Usecases.Imps;

/// <summary>
/// 顧客アカウント登録Usecaseの実装
/// </summary>
public class RegisterCustomerUsecase : IRegisterCustomerUsecase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IPasswordService _passwordService;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="customerRepository">顧客Repository</param>
    /// <param name="passwordService">パスワード処理Service</param>
    public RegisterCustomerUsecase(
        ICustomerRepository customerRepository,
        IPasswordService passwordService)
    {
        _customerRepository = customerRepository;
        _passwordService = passwordService;
    }

    /// <summary>
    /// 顧客情報を登録する
    /// </summary>
    /// <param name="input">顧客アカウント登録情報</param>
    /// <returns>登録した顧客のUUID</returns>
    /// <exception cref="ConflictException">
    /// メールアドレス、アカウント名、電話番号のいずれかが
    /// 既に登録されている場合
    /// </exception>
    public async Task<string> ExecuteAsync(
        (
            string Name,
            string NameKana,
            string Address1,
            string Address2,
            string PhoneNumber,
            string MailAddress,
            string Username,
            string Password
        ) input)
    {
        var customerByMailAddress =
            await _customerRepository.FindByMailAddressAsync(
                input.MailAddress);

        var customerByUsername =
            await _customerRepository.FindByUsernameAsync(
                input.Username);

        var customerByPhoneNumber =
            await _customerRepository.FindByPhoneNumberAsync(
                input.PhoneNumber);

        if (customerByMailAddress != null ||
            customerByUsername != null ||
            customerByPhoneNumber != null)
        {
            throw new ConflictException(
                "このアカウント名、メールアドレス、または電話番号は既に登録されています。");
        }

        var passwordHash =
            _passwordService.Hash(input.Password);

        var customer = Customer.Create(
            input.Name,
            input.NameKana,
            input.Address1,
            input.Address2,
            input.PhoneNumber,
            input.MailAddress,
            input.Username,
            input.Password,
            passwordHash);

        return await _customerRepository.CreateAsync(customer);
    }
}