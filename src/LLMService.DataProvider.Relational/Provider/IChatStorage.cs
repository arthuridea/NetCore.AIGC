using LLMService.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.DataProvider.Relational.Provider
{
    internal interface IChatStorage<TChatMessage, TMessageContent>
        where TChatMessage : IChatMessage<TMessageContent>, new()
    {
    }
}
