using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jurumani.BotBuilder.Models;
using Jurumani.BotBuilder.Utils;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jurumani.BotBuilder.Dialogs
{
    
  
    public class WifiDialog : ComponentDialog
    {
        private static HttpClient _httpClient = new HttpClient();
        private List<ProductModel> _wifimodels;
    

        private QuoteBotAccessors _botAccessors;
        public WifiDialog(QuoteBotAccessors quoteBotAccessors) : base(nameof(WifiDialog))
        {
            _botAccessors = quoteBotAccessors;
            using (StreamReader r = new StreamReader("Datasuitcase/WifiModels.json"))
            {
                string json = r.ReadToEnd();
                _wifimodels = JsonConvert.DeserializeObject<List<ProductModel>>(json);
            }
            AddDialog(new AddMoreDevicesDialog(_botAccessors));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
            AddDialog(new WaterfallDialog("wifiDialog", new WaterfallStep[] {

              WifiModelStepAsync,
               WifiQuantityStepAsync,
               WifiSummaryStepAsync,

            }));
            InitialDialogId = "wifiDialog";
            AddDialog(new AddMoreDevicesDialog(_botAccessors));
        }
        


        private async Task<DialogTurnResult> WifiModelStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var actionlist = new List<CardAction>() { };
            _wifimodels.ForEach(model =>
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
           // var res=await WebServicesFactory.QueryProductData("MS120-24-HW", "LIC");
           // await stepContext.Context.SendActivityAsync(MessageFactory.Text(res.ToString()));
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = response


            });

        }
        

        private async Task<DialogTurnResult> WifiQuantityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var wifimodel = (string)stepContext.Result;
            stepContext.Values["wifimodel"] = wifimodel;

            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
            {
                Prompt = MessageFactory.Text($"How many {wifimodel} would you like?")
            });
        }
        private async Task<DialogTurnResult> WifiSummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var _wifiProduct = new BitrixProductRowModel();
            var title = (string)stepContext.Values["wifimodel"];
               var  value = (string)stepContext.Values["wifimodel"];
            var price = float.Parse(stepContext.Result.ToString());
           
            await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
            {
                Prompt = MessageFactory.Text($"You ordered {stepContext.Result}x {stepContext.Values["wifimodel"]}")
            });
            var Basket =await _botAccessors.QuoteBasket.GetAsync(stepContext.Context, () => new QuoteBasketModel(), cancellationToken);
            Basket.products.Add(_wifiProduct);
            var convostate= _botAccessors.ConversationState.LoadAsync(stepContext.Context, false,cancellationToken);
            
            await _botAccessors.QuoteBasket.SetAsync(stepContext.Context, Basket, cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(Basket.ToString()));
            return await stepContext.ReplaceDialogAsync(nameof(AddMoreDevicesDialog),Basket);
                }

        }

    

       

        
    

}