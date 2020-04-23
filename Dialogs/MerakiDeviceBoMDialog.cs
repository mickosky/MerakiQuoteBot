using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jurumani.BotBuilder.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Jurumani.BotBuilder.Dialogs
{
 
    public class MerakiDeviceBoMDialog:ComponentDialog
    {
       
        private QuoteBotAccessors _botAccessors;
        
       
        private static List<CardAction> _Devices = new List<CardAction>() {
            new CardAction(){
                Title="Wi-Fi ",
                Type=ActionTypes.ImBack,
                Value="wifi" },
           new CardAction(){
               Title="Firewall ",
               Type=ActionTypes.ImBack,
               Value="firewall"},
            new CardAction()
            {
                Title="Camera",
                Value="camera",
                Type=ActionTypes.ImBack,

            },
             new CardAction()
            {
                Title="Switch",
                Value="switch",
                Type=ActionTypes.ImBack
            }

            
       };
        public MerakiDeviceBoMDialog(QuoteBotAccessors botAccessors):base(nameof(MerakiDeviceBoMDialog))
        {
            /*
             * display device options to the customer
             * depending on which choice is made invoke different device conversations
             */

            _botAccessors = botAccessors;
            //AddDialog(new FirewallDialog());
            // AddDialog(new CameraDialog());
          
        AddDialog(new WaterfallDialog("DeviceBOMDialog", new WaterfallStep[]
        {  
            DeviceOptionsListAsync,
            DeviceSpecsStepAsync,
        
            //SaveDeviceStepAsync,
        }));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WifiDialog(_botAccessors));
            AddDialog(new SwitchDialog(_botAccessors));
            AddDialog(new FirewallDialog(_botAccessors));
            AddDialog(new CameraDialog(_botAccessors));
            AddDialog(new AddMoreDevicesDialog(_botAccessors));
            
        }
       

        private async Task<DialogTurnResult> DeviceOptionsListAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
                  
           
            var cacheUserProfile =await _botAccessors.QuoteBasket.GetAsync(stepContext.Context,(()=>new QuoteBasketModel()));
            
           // await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Hi {username.ToString()}"));
           // stepContext.Values["licenseduration"] = stepContext.Result.ToString();
            stepContext.Values["selecteddevice"] = "";
            var response = MessageFactory.Text("Please select a device Type");
            response.SuggestedActions = new SuggestedActions()
            {
                Actions =_Devices,
            };
         return await stepContext.PromptAsync(nameof(TextPrompt),new PromptOptions { Prompt = response }, cancellationToken);
           
           
    }
        private async Task<DialogTurnResult> DeviceSpecsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            
            var result = (string)stepContext.Values["selecteddevice"];
            result = (string)stepContext.Result;
            switch (result)
            {
                case "wifi":
                  
                    return await stepContext.ReplaceDialogAsync(nameof(WifiDialog));
                   
                case "switch":
                    return await stepContext.ReplaceDialogAsync(nameof(SwitchDialog));
                    
                case "firewall":
                    return await stepContext.Parent.ReplaceDialogAsync(nameof(FirewallDialog));
                    
                case "camera":
                    return await stepContext.ReplaceDialogAsync(nameof(CameraDialog));
               
                
            }
            


            await stepContext.Context.SendActivityAsync(result);
            return await stepContext.ContinueDialogAsync(cancellationToken);
        }
       
        


    }
}
 