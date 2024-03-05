using LLMService.DataProvider.Relational.Entity;
using LLMService.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.DataProvider.Relational.Provider
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TChatMessage">The type of the chat message.</typeparam>
    /// <typeparam name="TMessageContent">The type of the message content.</typeparam>
    /// <typeparam name="TChatDbContext">The type of the chat database context.</typeparam>
    public class ChatStorage<TChatMessage, TMessageContent, TChatDbContext> : IChatStorage<TChatMessage, TMessageContent>
        where TChatMessage : IChatMessage<TMessageContent>, new()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TChatMessage">The type of the chat message.</typeparam>
    /// <typeparam name="TMessageContent">The type of the message content.</typeparam>
    /// <typeparam name="TChatFolder">The type of the chat folder.</typeparam>
    /// <typeparam name="TConversation">The type of the conversation.</typeparam>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <typeparam name="TTenantKey">The type of the tenant key.</typeparam>
    /// <typeparam name="TUserKey">The type of the user key.</typeparam>
    /// <typeparam name="TConversationKey">The type of the conversation key.</typeparam>
    /// <typeparam name="TMessageKey">The type of the message key.</typeparam>
    /// <seealso cref="IChatStorage{TChatMessage, TMessageContent}" />
    public class ChatStorage<TChatMessage, TMessageContent, 
        TChatFolder, TConversation, TMessage, TTenantKey, TUserKey, TConversationKey, TMessageKey> : IChatStorage<TChatMessage, TMessageContent>
        where TChatMessage : IChatMessage<TMessageContent>, new()
        where TUserKey : IEquatable<TUserKey>
        where TTenantKey : IEquatable<TTenantKey>
        where TMessageKey : IEquatable<TMessageKey>
        where TConversationKey : IEquatable<TConversationKey>
        where TChatFolder : ChatFolder<TUserKey, TTenantKey>
        where TConversation : Conversation<TConversationKey, TTenantKey, TUserKey>
        where TMessage : ChatMessage<TMessageKey, TConversationKey, TTenantKey, TUserKey>
    {
    }
}
