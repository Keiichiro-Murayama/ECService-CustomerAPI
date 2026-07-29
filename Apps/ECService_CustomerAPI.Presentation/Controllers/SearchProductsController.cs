using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Presentation.Adapters;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ECService_CustomerAPI.Presentation.Controllers;

/// <summary>
/// 商品検索APIを提供するController
/// </summary>
[ApiController]
[Route("/api/customer/products")]
[Tags("商品検索")]
public class SearchProductsController : ControllerBase
{
    private readonly ISearchProductsUsecase
        _searchProductsUsecase;

    private readonly SearchProductsViewModelAdapter
        _searchProductsViewModelAdapter;

    private readonly ILogger<SearchProductsController>
        _logger;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="searchProductsUsecase">
    /// 商品検索Usecase
    /// </param>
    /// <param name="searchProductsViewModelAdapter">
    /// 商品検索ViewModelAdapter
    /// </param>
    /// <param name="logger">
    /// ログ出力
    /// </param>
    public SearchProductsController(
        ISearchProductsUsecase searchProductsUsecase,
        SearchProductsViewModelAdapter
            searchProductsViewModelAdapter,
        ILogger<SearchProductsController> logger)
    {
        _searchProductsUsecase =
            searchProductsUsecase;

        _searchProductsViewModelAdapter =
            searchProductsViewModelAdapter;

        _logger = logger;
    }

    /// <summary>
    /// カテゴリUUIDを条件に商品を検索する
    /// </summary>
    /// <param name="categoryUuid">
    /// カテゴリUUID。
    /// 未指定の場合は全商品を取得する。
    /// </param>
    /// <returns>商品一覧</returns>
    [HttpGet]
    [ProducesResponseType(
        typeof(List<ProductResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ProductResponse>>>
        SearchProductsAsync(
            [FromQuery] string? categoryUuid)
    {
        var normalizedCategoryUuid =
            categoryUuid?.Trim();

        /*
         * カテゴリUUIDが指定されている場合、
         * UUID形式を検証する
         */
        if (!string.IsNullOrWhiteSpace(
                normalizedCategoryUuid) &&
            !Guid.TryParse(
                normalizedCategoryUuid,
                out _))
        {
            return BadRequest(new
            {
                message =
                    "カテゴリUUIDの形式が" +
                    "正しくありません。"
            });
        }

        try
        {
            /*
             * 1. 商品を検索する
             */
            var products =
                await _searchProductsUsecase.ExecuteAsync(
                    normalizedCategoryUuid);

            /*
             * 2. ドメインオブジェクトを
             *    レスポンス形式へ変換する
             */
            var responses =
                await _searchProductsViewModelAdapter
                    .ConvertAsync(products);

            /*
             * 3. 200 OKを返す
             */
            return Ok(responses);
        }
        catch (DomainException ex)
        {
            /*
             * UUID形式は正しいが、
             * 指定カテゴリが存在しない場合
             */
            _logger.LogWarning(
                ex,
                "存在しないカテゴリUUIDが" +
                "指定されました。");

            return NotFound(new
            {
                message =
                    "指定されたカテゴリID（UUID）が" +
                    "存在しません。"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "商品検索処理でエラーが発生しました。");

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