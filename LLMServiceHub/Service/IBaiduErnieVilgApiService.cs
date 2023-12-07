using LLMServiceHub.Models;

namespace LLMServiceHub.Service
{
    /// <summary>
    /// 百度智能绘图api
    /// </summary>
    public interface IBaiduErnieVilgApiService
    {
        /// <summary>
        /// 文生图
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        Task<PaintResultResponse> Text2ImgV2(PaintApplyRequest request);
    }
}
