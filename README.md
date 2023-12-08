# 百度文心千帆大模型（文心一言、智能作画）SDK (.net6+)

![.NET6.0](https://badgen.net/badge/.NET/6.0/green)
![.NET8.0](https://badgen.net/badge/.NET/8.0/green)
![license](https://badgen.net/badge/license/mit)

`LLMService` 是一个非百度官方的开源项目.提供了接口方法能够在`.NET6.0` `.NET8.0`项目中使用接口调用百度文心大模型和百度智能绘图高级版功能.

## 安装

通过`Nuget`安装.

### 文心大模型

```
Install-Package Aeex.LLMService.Baidu.Wenxin
```

### 智能绘图V2

```
Install-Package Aeex.LLMService.Baidu.ErnieVilg
```

## 使用

1. 在`ConfigureService`方法中添加依赖注入

```
public void ConfigureServices(IServiceCollection services, IConfiguration config){
    //....
    
    // Ensure HttpContext injected. It will be accessed in Chat service.
    services.AddHttpContextAccessor();

    // inject image storage provider
    services.AddTransient<IImageStorageProvider, LocalImageStorageProvider>();
    // inject chat data provider
    services.AddTransient<IChatDataProvider<BaiduWenxinMessage>, ChatDataProvider<BaiduWenxinMessage>>();
    
    // inject baidu api service
    services.AddWenxinworkshop(config);
    services.AddErnieVilg(config);

    //...
}
```

2. Have fun! 

```

    /// <summary>
    /// 集成百度AI类api
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/baidu")]
    [ApiExplorerSettings(GroupName = "百度大模型")]
    public class BaiduApiController(
        IAIChatApiService<ChatRequest, ChatApiResponse> baiduApiService,
        IAIPaintApiService<PaintApplyRequest, PaintResultResponse> ernieVilgApiService) : ControllerBase
    {
        private readonly IAIChatApiService<ChatRequest, ChatApiResponse> _apiService = baiduApiService;
        private readonly IAIPaintApiService<PaintApplyRequest, PaintResultResponse> _ernieVilgApiService = ernieVilgApiService;

        /// <summary>
        /// 发起文心一言大模型对话
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost("chat")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(ChatApiResponse), 200)]
        public async Task Chat(ChatRequest request)
        {
            await _apiService.Chat(request);
        }

        /// <summary>
        /// 文生图
        /// <para>封装文生图api以及回调获取生成图片api</para>
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost("text2img")]
        [ProducesResponseType(typeof(PaintResultResponse), 200)]
        public async Task<IActionResult> ApplyText2Img(PaintApplyRequest request)
        {
            var result = await _ernieVilgApiService.Text2Image(request);
            Response.BuildAIGeneratedResponseFeature();
            return Ok(result);
        }
    }

```



## 支持的模型

`LLMService` 支持的语言模型列表如下：

| 模型 | 描述 |
| --- | --- |
| ERNIEBot | 百度自行研发的大语言模型，覆盖海量中文数据，具有更强的对话问答、内容创作生成等能力。 |
| ERNIEBotTurbo | 百度自行研发的大语言模型，覆盖海量中文数据，具有更强的对话问答、内容创作生成等能力，响应速度更快。 |
| ERNIE-Bot-4 | ERNIE-Bot-4是百度自行研发的大语言模型，覆盖海量中文数据，具有更强的对话问答、内容创作生成等能力。 |

## 项目说明

| 项目 | 说明 |
| --- | --- |
| `LLMService.Shared` | 公共模型和接口及扩展方法 |
| `LLMService.Baidu.WenxinWorkshop` | 百度千帆大模型项目，目前提供ErnieBot系列API调用支持 |
| `LLMService.Baidu.ErnieVilg` | 百度智能创作模块，目前支持AI作画高级版2 |
| `LLMServiceHub` | `.NET8 MVC WebApi` 项目示例，填入自己的 `application id`可以完美调用，实现了基本的接口 |