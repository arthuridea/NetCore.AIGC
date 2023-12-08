# 百度文心千帆大模型（文心一言、智能作画）SDK (.net6+)

![.NET6.0](https://badgen.net/badge/.NET/6.0/green)
![.NET8.0](https://badgen.net/badge/.NET/8.0/green)
![license](https://badgen.net/badge/license/mit)

`LLMService` 是一个非百度官方的开源项目.提供了接口方法能够在`.NET6.0` `.NET8.0`项目中使用接口调用百度文心大模型和百度智能绘图高级版功能.

## 项目说明

| 项目 | 说明 |
| --- | --- |
| `LLMService.Shared` | 公共模型和接口及扩展方法 |
| `LLMService.Baidu.WenxinWorkshop` | 百度千帆大模型项目，目前提供ErnieBot系列API调用支持 |
| `LLMService.Baidu.ErnieVilg` | 百度智能创作模块，目前支持AI作画高级版2 |
| `LLMServiceHub` | `.NET8 MVC WebApi` 项目示例，填入自己的 `application id`可以完美调用，实现了基本的接口 |

## 安装

## 使用

## 支持的模型

`LLMService` 支持的语言模型列表如下：

| 模型 | 描述 |
| --- | --- |
| ERNIEBot | 百度自行研发的大语言模型，覆盖海量中文数据，具有更强的对话问答、内容创作生成等能力。 |
| ERNIEBotTurbo | 百度自行研发的大语言模型，覆盖海量中文数据，具有更强的对话问答、内容创作生成等能力，响应速度更快。 |
| ERNIE-Bot-4 | ERNIE-Bot-4是百度自行研发的大语言模型，覆盖海量中文数据，具有更强的对话问答、内容创作生成等能力。 |
