using LLMService.Shared.Models;

namespace LLMService.DataProvider.Relational.Provider
{
    internal interface IChatStorage<TChatMessage, TMessageContent>
        where TChatMessage : IChatMessage<TMessageContent>, new()
    {
    }
}
