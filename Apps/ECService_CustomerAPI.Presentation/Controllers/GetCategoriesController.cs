using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Presentation.Adapters;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ECService_CustomerAPI.Presentation.Controllers;

/// <summary>
/// 商品カテゴリ一覧取得APIを提供するController
/// </summary>
[ApiController]
[Route("/api/customer/categories")]
[Tags("商品カテゴリ")]
public class GetCategoriesController : ControllerBase
{
    private readonly IGetCategoriesUsecase
        _getCategoriesUsecase;

    private readonly GetCategoriesViewModelAdapter
        _getCategoriesViewModelAdapter;

    private readonly ILogger<GetCategoriesController>
        _logger;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="getCategoriesUsecase">
    /// 商品カテゴリ一覧取得Usecase
    /// </param>
    /// <param name="getCategoriesViewModelAdapter">
    /// 商品カテゴリViewModelAdapter
    /// </param>
    /// <param name="logger">
    /// ログ出力
    /// </param>
    public GetCategoriesController(
        IGetCategoriesUsecase getCategoriesUsecase,
        GetCategoriesViewModelAdapter
            getCategoriesViewModelAdapter,
        ILogger<GetCategoriesController> logger)
    {
        _getCategoriesUsecase =
            getCategoriesUsecase;

        _getCategoriesViewModelAdapter =
            getCategoriesViewModelAdapter;

        _logger = logger;
    }

    /// <summary>
    /// 商品カテゴリ一覧を取得する
    /// </summary>
    /// <returns>商品カテゴリ一覧</returns>
    [HttpGet]
    [ProducesResponseType(
        typeof(List<CategoryResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<CategoryResponse>>>
        GetCategoriesAsync()
    {
        try
        {
            /*
             * 1. 商品カテゴリ一覧を取得する
             */
            var categories =
                await _getCategoriesUsecase.ExecuteAsync();

            /*
             * 2. ドメインオブジェクトを
             *    レスポンス形式へ変換する
             */
            var responses =
                await _getCategoriesViewModelAdapter
                    .ConvertAsync(categories);

            /*
             * 3. 200 OKを返す
             */
            return Ok(responses);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "商品カテゴリ一覧取得処理で" +
                "エラーが発生しました。");

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