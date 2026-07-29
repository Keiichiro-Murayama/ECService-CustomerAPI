using ECService_CustomerAPI.Application.Exceptions;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Presentation.Adapters;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ECService_CustomerAPI.Presentation.Controllers;

/// <summary>
/// 商品詳細取得APIを提供するController
/// </summary>
[ApiController]
[Route("/api/customer/products")]
[Tags("商品詳細")]
public class GetProductDetailController : ControllerBase
{
    private readonly IGetProductDetailUsecase
        _getProductDetailUsecase;

    private readonly GetProductDetailViewModelAdapter
        _getProductDetailViewModelAdapter;

    private readonly ILogger<GetProductDetailController>
        _logger;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="getProductDetailUsecase">
    /// 商品詳細取得Usecase
    /// </param>
    /// <param name="getProductDetailViewModelAdapter">
    /// 商品詳細ViewModelAdapter
    /// </param>
    /// <param name="logger">
    /// ログ出力
    /// </param>
    public GetProductDetailController(
        IGetProductDetailUsecase getProductDetailUsecase,
        GetProductDetailViewModelAdapter
            getProductDetailViewModelAdapter,
        ILogger<GetProductDetailController> logger)
    {
        _getProductDetailUsecase =
            getProductDetailUsecase;

        _getProductDetailViewModelAdapter =
            getProductDetailViewModelAdapter;

        _logger = logger;
    }

    /// <summary>
    /// 商品UUIDから商品詳細を取得する
    /// </summary>
    /// <param name="productUuid">商品UUID</param>
    /// <returns>商品詳細</returns>
    [HttpGet("{productUuid}")]
    [ProducesResponseType(
        typeof(ProductDetailResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductDetailResponse>>
        GetProductDetailAsync(
            string productUuid)
    {
        try
        {
            /*
             * 1. 商品詳細を取得する
             */
            var productDetail =
                await _getProductDetailUsecase
                    .ExecuteAsync(productUuid);

            /*
             * 2. レスポンス形式へ変換する
             */
            var response =
                await _getProductDetailViewModelAdapter
                    .ConvertAsync(productDetail);

            /*
             * 3. 200 OKを返す
             */
            return Ok(response);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(
                ex,
                "不正な商品UUIDが指定されました。");

            return BadRequest(new
            {
                message =
                    "商品UUIDの形式が正しくありません。"
            });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(
                ex,
                "指定された商品が見つかりませんでした。");

            return NotFound(new
            {
                message =
                    "指定された商品が存在しません。"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "商品詳細取得処理でエラーが発生しました。");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "InternalException: " +
                        "サーバー内部で予期せぬ" +
                        "エラーが発生しました。"
                });
        }
    }
}