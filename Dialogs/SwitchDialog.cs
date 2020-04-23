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
    
     public class SwitchDialog : ComponentDialog
    {
        private List<ProductModel> _switchmodels;

        private QuoteBotAccessors _botAccessors;
        public SwitchDialog(QuoteBotAccessors botAccessors)
        {
            _botAccessors = botAccessors;
            using(StreamReader r = new StreamReader("DataSuitCase/SwitchModels.json"))
            {
                string json = r.ReadToEnd();
                _switchmodels = JsonConvert.DeserializeObject<List<ProductModel>>(json);
            }
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
            AddDialog(new WaterfallDialog("switchDialog", new WaterfallStep[] {

              SwitchModelStepAsync,
               SwitchQuantityStepAsync,
               SwitchSummaryStepAsync,
            }));
            InitialDialogId = "switchDialog";
        }
        private async Task<DialogTurnResult> SwitchModelStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var actionlist = new List<CardAction>() { };
            _switchmodels.ForEach(model =>
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

        private async Task<DialogTurnResult> SwitchQuantityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var switchmodel = (string)stepContext.Result;
            stepContext.Values["switchmodel"] = switchmodel;

            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
            {
                Prompt = MessageFactory.Text($"How many {switchmodel} would you like?")
            });
        }
        private async Task<DialogTurnResult> SwitchSummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var _switchmodel = new BitrixProductRowModel();
           var title = stepContext.Values["switchmodel"].ToString();
            var value = stepContext.Values["switchmodel"].ToString();
           var price = float.Parse(stepContext.Result.ToString());
            await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
            {
                Prompt = MessageFactory.Text($"You ordered {stepContext.Result}x {stepContext.Values["switchmodel"]}s")
            });
            var Basket = await _botAccessors.QuoteBasket.GetAsync(stepContext.Context, () => new QuoteBasketModel(), cancellationToken);
            Basket.products.Add(_switchmodel);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(Basket.ToString()));
            return await stepContext.ReplaceDialogAsync(nameof(AddMoreDevicesDialog));
        }

    }


}
