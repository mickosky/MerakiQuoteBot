using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Jurumani.BotBuilder.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading.Tasks;
using System.Threading;
using Jurumani.BotBuilder.Utils;
using System.Net.Http;


namespace Jurumani.BotBuilder.Dialogs
{
    public class CustomerProfileDialog:ComponentDialog
    {

        private QuoteBotAccessors _botaccessors;
       
        private const string customerInfo ="value-customerInfo";
        public CustomerProfileDialog(QuoteBotAccessors quoteBotAccessors):base(nameof(CustomerProfileDialog)) 
        {
            _botaccessors = quoteBotAccessors;
            var customerCaptureSteps = new WaterfallStep[]
         {
             PhoneStepAsync,
             NameStepAsync,
             LastNameStepAsync,
             EmailStepAsync,
             CompanyStepAsync,
             JumpTopDeviceOptionsStepAsync,

          };
            
           
           
            //AddDialog(new MerakiDeviceBoMDialog());
            AddDialog(new WaterfallDialog("CustomerProfileDialog", customerCaptureSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
           
           
        }

        private async Task<DialogTurnResult> PhoneStepAsync(WaterfallStepContext stepContext,CancellationToken cancellationToken)
        {
            
            stepContext.Values[customerInfo] = new CustomerModel();
            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions { Prompt = MessageFactory.Text
                ("Please enter your Phone Number") }, cancellationToken);
        }
       private async Task<DialogTurnResult>NameStepAsync(WaterfallStepContext stepContext,CancellationToken cancellation)
        {
           
           var customerProfile = (CustomerModel)stepContext.Values[customerInfo];
            var customerDetails = CustomerUtil.getCustomerByPhoneNumber(stepContext.Result.ToString());
            if (customerDetails.Count > 0){
                customerProfile = customerDetails[0];
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Hi {customerDetails[0].NAME} {customerDetails[0].LAST_NAME}  company id is {customerDetails[0].COMPANY_ID}" ));
                customerProfile.NAME = customerDetails[0].NAME;
                
                var cacheCustomerInfo =await _botaccessors.UserProfile.GetAsync(stepContext.Context, () => new CustomerModel());
                cacheCustomerInfo.NAME = customerDetails[0].NAME;
                cacheCustomerInfo.LAST_NAME = customerDetails[0].LAST_NAME;
                cacheCustomerInfo.PHONE = customerDetails[0].PHONE;
                cacheCustomerInfo.COMPANY_ID = customerDetails[0].COMPANY_ID;
                cacheCustomerInfo.EMAIL = customerDetails[0].EMAIL;
                
                await _botaccessors.UserProfile.SetAsync(stepContext.Context,cacheCustomerInfo,cancellation);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"saved company id {cacheCustomerInfo.COMPANY_ID}"));
                return await stepContext.ReplaceDialogAsync(nameof(MerakiDeviceBoMDialog));
            }
            else
            {
               
                var phonegeneric = new Generics();
                phonegeneric.VALUE = stepContext.Result.ToString();
                List<Generics> PHONE = new List<Generics>() { phonegeneric };
               
                var custdata = (CustomerModel) stepContext.Values[customerInfo];
                custdata.PHONE = PHONE;
                var cacheCustomerProfile = await _botaccessors.UserProfile.GetAsync(stepContext.Context, () => new CustomerModel(), cancellation);
                cacheCustomerProfile =  custdata;
                await _botaccessors.UserProfile.SetAsync(stepContext.Context, cacheCustomerProfile, cancellation);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter your Name") }, cancellation);
            }
            
        }
        private async Task<DialogTurnResult> LastNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            var customerdata = (CustomerModel)stepContext.Values[customerInfo];
            if (stepContext.Result.ToString() != null)
            {
                customerdata.NAME = stepContext.Result.ToString();

            }
           
            var cacheCustomerProfile = await _botaccessors.UserProfile.GetAsync(stepContext.Context, () => new CustomerModel(), cancellation);
            cacheCustomerProfile = customerdata;
            await _botaccessors.UserProfile.SetAsync(stepContext.Context, cacheCustomerProfile, cancellation);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter your Surname") }, cancellation);
        }

        private async Task<DialogTurnResult> EmailStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            var customerdata = (CustomerModel)stepContext.Values[customerInfo];
            customerdata.LAST_NAME = stepContext.Result.ToString();
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("What is your email address") }, cancellation);
        }

        private async Task<DialogTurnResult>CompanyStepAsync(WaterfallStepContext stepContext,CancellationToken cancellationtoken)
        {
            var customerdata = (CustomerModel)stepContext.Values[customerInfo];
            var emailgeneric = new Generics();
            if (stepContext.Result.ToString() != null){
                emailgeneric.VALUE = stepContext.Result.ToString();
            }
             
            List<Generics> EMAIL = new List<Generics>() { emailgeneric };
            customerdata.EMAIL = EMAIL;
         
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter your company name")
            }) ;
        }

        private async Task<DialogTurnResult> JumpTopDeviceOptionsStepAsync(WaterfallStepContext stepContext,CancellationToken cancellationtoken)
        {
            var customerdata = (CustomerModel)stepContext.Values[customerInfo];
            var company_name = "";
          if (stepContext.Result.ToString()!=null)
                
            {
              company_name  = stepContext.Result.ToString();
            }
      
            
            var companyID = CompanyUtil.SaveCompanyDetails(company_name);
            customerdata.COMPANY_ID = companyID;
           var customerID = CustomerUtil.saveCustomerInformation(customerdata.NAME, customerdata.LAST_NAME,customerdata.COMPANY_ID,customerdata.EMAIL[0].VALUE,customerdata.PHONE[0].VALUE);
               await stepContext.Context.SendActivityAsync(MessageFactory.Text($"The company id is {companyID} "));


            var cacheCustomerInfo = await _botaccessors.UserProfile.GetAsync(stepContext.Context, ()=>new CustomerModel());
            customerdata.ID = customerID;
           
            cacheCustomerInfo = customerdata;
            
            await _botaccessors.UserProfile.SetAsync(stepContext.Context,cacheCustomerInfo,cancellationtoken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(customerdata.ToString()));


           return await stepContext.ReplaceDialogAsync(nameof(MerakiDeviceBoMDialog),cacheCustomerInfo);

        }
       

       
    }
}
