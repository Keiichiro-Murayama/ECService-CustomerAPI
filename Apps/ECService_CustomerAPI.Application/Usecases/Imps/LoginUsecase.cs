using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Application.Authentications;
using ECService_CustomerAPI.Application.Exceptions;
using ECService_CustomerAPI.Domain.Repositories;

namespace ECService_CustomerAPI.Application.Usecases.Imps;

/// <summary>
/// ログインユースケースの実装
/// メールアドレスで顧客を検索し、パスワードを検証してJWTトークンを生成する。
/// 認証に成功すればJWTを返し、失敗すれば例外をスローする。
/// 読み取り専用のユースケースであり、トランザクションは不要。
/// </summary>
public class LoginUsecase : ILoginUsecase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtTokenProvider _jwtTokenProvider;

    public LoginUsecase(
        ICustomerRepository customerRepository,
        IPasswordService passwordService,
        IJwtTokenProvider jwtTokenProvider)
    {
        _customerRepository = customerRepository;
        _passwordService = passwordService;
        _jwtTokenProvider = jwtTokenProvider;
    }

    public async Task<(string AccessToken, Customer Customer)> ExecuteAsync((string MailAddress, string Password) input)
    {
        var customer = await _customerRepository.FindByMailAddressAsync(input.MailAddress);
        if (customer == null)
        {
            throw new AuthenticationException("AuthenticationFailed", "メールアドレスまたはパスワードが正しくありません。");
        }

        if (!_passwordService.Verify(input.Password, customer.PasswordHash))
        {
            throw new AuthenticationException("AuthenticationFailed", "メールアドレスまたはパスワードが正しくありません。");
        }

        var accessToken = _jwtTokenProvider.IssueAccessToken(customer);
        return (accessToken, customer);
    }
}