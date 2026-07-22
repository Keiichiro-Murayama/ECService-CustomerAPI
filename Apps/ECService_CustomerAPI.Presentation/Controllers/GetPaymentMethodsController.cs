using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Presentation.Adapters;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECService_CustomerAPI.Presentation.Controllers;

/// <summary>
/// 支払い方法一覧取得APIを提供するController
/// </summary>
[ApiController]
[Route("/api/customer/payments")]
[Tags("支払い方法")]
public class GetPaymentMethodsController : ControllerBase
{
    private readonly IGetPaymentMethodsUsecase
        _getPaymentMethodsUsecase;

    private readonly GetPaymentMethodsViewModelAdapter
        _getPaymentMethodsViewModelAdapter;

    private readonly ILogger<GetPaymentMethodsController>
        _logger;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="getPaymentMethodsUsecase">
    /// 支払い方法一覧取得Usecase
    /// </param>
    /// <param name="getPaymentMethodsViewModelAdapter">
    /// 支払い方法ViewModelAdapter
    /// </param>
    /// <param name="logger">
    /// ログ出力
    /// </param>
    public GetPaymentMethodsController(
        IGetPaymentMethodsUsecase getPaymentMethodsUsecase,
        GetPaymentMethodsViewModelAdapter
            getPaymentMethodsViewModelAdapter,
        ILogger<GetPaymentMethodsController> logger)
    {
        _getPaymentMethodsUsecase =
            getPaymentMethodsUsecase;

        _getPaymentMethodsViewModelAdapter =
            getPaymentMethodsViewModelAdapter;

        _logger = logger;
    }

    /// <summary>
    /// 支払い方法一覧を取得する
    /// </summary>
    /// <returns>支払い方法一覧</returns>
    [HttpGet]
    [ProducesResponseType(
        typeof(List<PaymentMethodResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<PaymentMethodResponse>>>
        GetPaymentMethodsAsync()
    {
        try
        {
            /*
             * 1. 支払い方法一覧を取得する
             */
            var paymentMethods =
                await _getPaymentMethodsUsecase.ExecuteAsync();

            /*
             * 2. ドメインオブジェクトを
             *    レスポンス形式へ変換する
             */
            var responses =
                await _getPaymentMethodsViewModelAdapter
                    .ConvertAsync(paymentMethods);

            /*
             * 3. 200 OKを返す
             */
            return Ok(responses);
        }
        catch (Exception ex)
        {
            /*
             * DB処理を含む予期しないエラー
             */
            _logger.LogError(
                ex,
                "支払い方法一覧取得処理でエラーが発生しました。");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "InternalException: " +
                        "サーバー内部で予期せぬエラーが発生しました。"
                });
        }
    }
}