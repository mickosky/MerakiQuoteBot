using System;
using Jurumani.BotBuilder.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
namespace Jurumani.BotBuilder.Models
{

    public class QuoteBotAccessors
    {
        public ConversationState ConversationState { get; }
        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }
        public IStatePropertyAccessor<CustomerModel> UserProfile { get; set; }
        public IStatePropertyAccessor<QuoteBasketModel> QuoteBasket { get; set; }
        public UserState UserState { get; }
        public QuoteBotAccessors(ConversationState conversationState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));

        }

       
        
    }
}
