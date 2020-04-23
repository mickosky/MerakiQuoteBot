using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Jurumani.BotBuilder.Dialogs
{
    public class IntroDialog : ComponentDialog
    {
    
        // private IStatePropertyAccessor<IntroDialog> _introPropertyAccessor;
        public IntroDialog() : base(nameof(IntroDialog))
        {
            
            AddDialog(new WaterfallDialog("IntroDialog", new WaterfallStep[]
            {
                GreetingStepAsync,
                LicenseDurationStepAsync,
                ContinueOrEndStepAsync

            }));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

        }
        private static async Task<DialogTurnResult> GreetingStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationtoken)
        {
            var response = MessageFactory.Text("Choose what you would like me to help you with:");
            response.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){Title="Create Quote",Type=ActionTypes.ImBack,Value="Quote" },
                    new CardAction(){Title="End Conversation",Type=ActionTypes.ImBack,Value="End"}
                },
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = response }, cancellationtoken);

        }
        private async Task<DialogTurnResult> LicenseDurationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationtoken)
        {
            var response = MessageFactory.Text("Please choose the duration for your license");
            response.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                  {
                      new CardAction()
                      {
                         Title="1YR",
                         Value="1YR",
                         Type=ActionTypes.ImBack,
                      },
                    new CardAction()
                      {
                         Title="3YR",
                         Value="3YR",
                         Type=ActionTypes.ImBack,
                      },
                    new CardAction()
                      {
                         Title="5YR",
                         Value="5YR",
                         Type=ActionTypes.ImBack,
                      }
                  }
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = response }, cancellationtoken);
        }
        private static async Task<DialogTurnResult> ContinueOrEndStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationtoken)
        {
            var response = stepContext.Result.ToString();
            if (response == "Quote")
            {
                
                return await stepContext.BeginDialogAsync(stepContext.Result.ToString(),nameof(MerakiDeviceBoMDialog));

            }
            else
            {
                return await stepContext.ContinueDialogAsync(cancellationtoken);
            }
        }
    }
}
