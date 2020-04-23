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
    public class CameraDialog : ComponentDialog
    {
        private List<CameraModel> _cameramodels;

        private QuoteBotAccessors _botaccessors;
        public CameraDialog(QuoteBotAccessors quoteBotAccessors )
        {
            _botaccessors = quoteBotAccessors;
            using (StreamReader r = new StreamReader("DataSuitCase/CameraModels.json"))
            {
                string json = r.ReadToEnd();
                _cameramodels = JsonConvert.DeserializeObject<List<CameraModel>>(json);
            }
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
            AddDialog(new WaterfallDialog("CameraDialog", new WaterfallStep[] {

              ModelStepAsync,
               QuantityStepAsync,
               SummaryStepAsync,
            }));
            InitialDialogId = "CameraDialog";
        }

        private async Task<DialogTurnResult> ModelStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var actionlist = new List<CardAction>() { };
            _cameramodels.ForEach(model =>
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

        private async Task<DialogTurnResult> QuantityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var cameramodel = (string)stepContext.Result;
            stepContext.Values["cameramodel"] = cameramodel;

            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
            {
                Prompt = MessageFactory.Text($"How many {cameramodel} would you like?")
            });
        }
        private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
           var cameramodel = new BitrixProductRowModel();
            var title = (string)stepContext.Values["cameramodel"];
        var price = float.Parse(stepContext.Result.ToString());
            var Basket=await _botaccessors.QuoteBasket.GetAsync(stepContext.Context,()=>new QuoteBasketModel(),cancellationToken);
            await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
            {
                Prompt = MessageFactory.Text($"You ordered {stepContext.Result}x {stepContext.Values["cameramodel"]}")
            });
            Basket.products.Add(cameramodel);
            await _botaccessors.QuoteBasket.SetAsync(stepContext.Context, Basket, cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(Basket.ToString()));
            return await stepContext.ReplaceDialogAsync(nameof(AddMoreDevicesDialog));
        }

    }


}
