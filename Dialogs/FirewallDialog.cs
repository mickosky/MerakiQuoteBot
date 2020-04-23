using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Jurumani.BotBuilder.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Jurumani.BotBuilder.Dialogs
{
    public class FirewallDialog : ComponentDialog
    {
        private List<ProductModel> _firewallmodels;
        private QuoteBotAccessors _botaccessors;
       
        public FirewallDialog(QuoteBotAccessors botAccessors)
        {
            _botaccessors = botAccessors;
           
            using (StreamReader r = new StreamReader("DataSuitCase/FireWallModels.json"))
            {
                string json = r.ReadToEnd();
                _firewallmodels = JsonConvert.DeserializeObject<List<ProductModel>>(json);
            }
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
           
            AddDialog(new WaterfallDialog("FirewallDialog", new WaterfallStep[] {
                FirewallModelStepAsync,
                FirewallQuantityStepAsync,
                FirewallSummaryStepAsync
            }));
            InitialDialogId = "FirewallDialog";
        }

        private async Task<DialogTurnResult> FirewallModelStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var actionlist = new List<CardAction>() { };
            _firewallmodels.ForEach(model =>
            {

                actionlist.Add(new CardAction
                {
                    Title = model.title,

                    Type = ActionTypes.ImBack,
                    Value = model.title

                });

            });

            var response = MessageFactory.Text($"Please select a model? ");
            response.SuggestedActions = new SuggestedActions()
            {
                Actions = actionlist


            };

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = response


            });
        }

        private async Task<DialogTurnResult> FirewallQuantityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var firewallmodel = (string)stepContext.Result;
            stepContext.Values["firewallmodel"] = firewallmodel;

            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
            {
                Prompt = MessageFactory.Text($"How many {firewallmodel} would you like?")
            });
        }
        private async Task<DialogTurnResult> FirewallSummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var _firewallmodel = new BitrixProductRowModel();
            var _title = (string)stepContext.Values["firewallmodel"];
            var _quantity= Int32.Parse(stepContext.Result.ToString());
            
            await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
            {
                Prompt = MessageFactory.Text($"You ordered {stepContext.Result}x {stepContext.Values["firewallmodel"]}")
            });
            var Basket = await _botaccessors.QuoteBasket.GetAsync(stepContext.Context, () => new QuoteBasketModel(), cancellationToken);
            Basket.products.Add(_firewallmodel);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(Basket.ToString()));
            return await stepContext.ReplaceDialogAsync(nameof(AddMoreDevicesDialog));
        }

    }


}