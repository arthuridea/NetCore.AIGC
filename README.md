# ChatGPT、文心一言、百度智能作画 SDK (.net6.0+)

![.NET6.0](https://badgen.net/badge/.NET/6.0/green)
![.NET8.0](https://badgen.net/badge/.NET/8.0/green)
![license](https://badgen.net/badge/license/mit)

`Aeex.LLMService` 是一个非官方的开源项目.提供了在`.NET6.0` `.NET8.0`项目中使用接口调用OpenAI ChatGPT、百度文心大模型和百度智能绘图高级版功能.

## 已知问题

## 安装

通过`Nuget`安装.

### OpenAI ChatGPT

[Nuget地址](https://www.nuget.org/packages/Aeex.LLMService.OpenAI.ChatGPT/)

#### Package Manager
``` bat
Install-Package Aeex.LLMService.OpenAI.ChatGPT
```
#### Cli
``` bat
dotnet add package Aeex.LLMService.OpenAI.ChatGPT
```

### 文心大模型

[Nuget地址](https://www.nuget.org/packages/Aeex.LLMService.Baidu.Wenxin/)

#### Package Manager
``` bat
Install-Package Aeex.LLMService.Baidu.Wenxin
```
#### Cli
``` bat
dotnet add package Aeex.LLMService.Baidu.Wenxin
```

### 智能绘图V2       

[Nuget地址](https://www.nuget.org/packages/Aeex.LLMService.Baidu.ErnieVilg/)

#### Package Manager

``` bat
Install-Package Aeex.LLMService.Baidu.ErnieVilg
```                                         
#### Cli
``` bat
dotnet add package Aeex.LLMService.Baidu.ErnieVilg
```

## 使用

1. 在`ConfigureService`方法中添加依赖注入

``` Csharp
public void ConfigureServices(IServiceCollection services, IConfiguration config){
    //....
    
    // Ensure HttpContext injected. It will be accessed in Chat service.
    services.AddHttpContextAccessor();

    // inject image storage provider
    services.AddTransient<IImageStorageProvider, LocalImageStorageProvider>();
    // inject chat data provider
    // for baidu chat caching
    services.AddTransient<IChatDataProvider<ChatMessageBase, string>, ChatDataProvider<ChatMessageBase, string>>();
    // for chatgpt
    services.AddTransient<IChatDataProvider<OpenAIChatMessage, List<OpenAIMessageContent>>, ChatDataProvider<OpenAIChatMessage, List<OpenAIMessageContent>>>();
    
    // inject service
    services.AddChatGPT(config);
    services.AddWenxinworkshop(config);
    services.AddErnieVilg(config);

    //...
}
```

2. Have fun! 

``` Csharp

    /// <summary>
    /// 集成百度AI类api
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/baidu")]
    public class BaiduApiController(
        IBaiduErniebotLLMService baiduApiService,
        IAIPaintApiService<PaintApplyRequest, PaintResultResponse> ernieVilgApiService) : ControllerBase
    {
        /// <summary>
        /// The API service
        /// </summary>
        private readonly IBaiduErniebotLLMService _apiService = baiduApiService;
        /// <summary>
        /// The ernie vilg API service
        /// </summary>
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

``` Csharp

    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/chatgpt")]
    [ApiExplorerSettings(GroupName = "ChatGPT")]
    public class ChatGPTApiController(
        IChatGPTLLMService gptService): ControllerBase
    {

        /// <summary>
        /// The API service
        /// </summary>
        private readonly IChatGPTLLMService _apiService = gptService;

        /// <summary>
        /// 发起ChatGPT对话
        /// eg.
        /// what is the difference between star and planet?
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost("chat")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(OpenAIChatResponse), 200)]
        [AppExceptionInterceptor(ReturnCode = -100001, ApiVersion = "1.0")]
        public async Task Chat(ChatRequest request)
        {
            await _apiService.Chat(request);
        }

    }
```



## 支持的模型

`Aeex.LLMService` 支持的语言模型列表如下：

| 模型 | 描述 |
| --- | --- |
| ERNIEBot | 百度自行研发的大语言模型，覆盖海量中文数据，具有更强的对话问答、内容创作生成等能力。 |
| ERNIEBotTurbo | 百度自行研发的大语言模型，覆盖海量中文数据，具有更强的对话问答、内容创作生成等能力，响应速度更快。 |
| ERNIE-Bot-4 | ERNIE-Bot-4是百度自行研发的大语言模型，覆盖海量中文数据，具有更强的对话问答、内容创作生成等能力。 |
| GPT_3_5_TURBO| |
| GPT_3_5_TURBO_1106| |
| GPT_4| |              
| GPT_4_32K| |

## 项目说明

| 项目 | 说明 |
| --- | --- |
| `LLMService.Shared` | 公共模型和接口及扩展方法 |            
| `LLMService.OpenAI.ChatGPT` | ChatGPT大模型项目，目前支持文字对话|
| `LLMService.Baidu.WenxinWorkshop` | 百度千帆大模型项目，目前提供ErnieBot系列API调用支持 |
| `LLMService.Baidu.ErnieVilg` | 百度智能创作模块，目前支持AI作画高级版2 |
| `LLMServiceHub` | `.NET8 MVC WebApi` 项目示例，填入自己的 `application id`可以完美调用，实现了基本的接口 |