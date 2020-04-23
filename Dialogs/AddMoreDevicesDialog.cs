using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jurumani.BotBuilder.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Jurumani.BotBuilder.Utils;
namespace Jurumani.BotBuilder.Dialogs
{
    public class AddMoreDevicesDialog : ComponentDialog
    {
        private QuoteBotAccessors _botaccessors;
        private static string DEALENDPOINT = "crm.deal.add";
        private static string QUOTEENDPOINT = "crm.quote.add";
        private static string TERMS = "Quotation valid for 7 days , and subject to an exchange current rand rate";
    
        public AddMoreDevicesDialog(QuoteBotAccessors botaccessors) : base(nameof(AddMoreDevicesDialog))
        {
            _botaccessors = botaccessors;
            AddDialog(new IntroDialog());
            AddDialog(new TextPrompt(nameof(TextPrompt)));
           
            AddDialog(new WaterfallDialog("AddMoreDialog", new WaterfallStep[] {
       
            EndOrRestartStepAsync,
             SaveOrContinueStepAsync
            }));
            InitialDialogId = "AddMoreDialog";
        }
        private async Task<DialogTurnResult> EndOrRestartStepAsync
           (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var response = MessageFactory.Text("Would you like to add another one?");
            response.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){
                        Title="Yes",
                        Value="yes",
                       Type=ActionTypes.ImBack
                    },
                    new CardAction(){
                        Title="No",
                        Value="no",
                       Type=ActionTypes.ImBack
                    }

                }
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = response }, cancellationToken);

        }
        private async Task<DialogTurnResult> SaveOrContinueStepAsync(WaterfallStepContext stepContext,CancellationToken cancellationtoken)
        {
            var answer = stepContext.Result.ToString();
            if (answer == "yes")
            {
                return await stepContext.ReplaceDialogAsync(nameof(MerakiDeviceBoMDialog));
            }
            else
            {
                //save all state
                var Basket =await _botaccessors.QuoteBasket.GetAsync(stepContext.Context, () => new QuoteBasketModel(), cancellationtoken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"company is id {Basket.customer.COMPANY_ID} customername is {Basket.customer.NAME}"));
                var customercompany = CompanyUtil.GetCompanyById(Basket.customer.COMPANY_ID);
                
                //var dealid=DealsUtil.CreateDeal($"testdeal-{customercompany.NAME}", Basket.customer.COMPANY_ID, Basket.customer.COMPANY_ID, DEALENDPOINT);
                //await stepContext.Context.SendActivityAsync(MessageFactory.Text("the deal is " + dealid));
                //var quoteexpirationdate = DateTime.Today.AddDays(7);
                //var quoteid = DealsUtil.CreateQuote(Basket.customer.NAME, Basket.customer.LAST_NAME, customercompany.NAME,dealid,Basket.customer.EMAIL[0].VALUE,quoteexpirationdate.ToString(),TERMS,QUOTEENDPOINT);
                //Basket.clearProducts();
                //await stepContext.Context.SendActivityAsync($"Check your emails for the quote {quoteid} ");
                return await stepContext.ReplaceDialogAsync(nameof(IntroDialog)) ;
            }
        }
        
    }
}
