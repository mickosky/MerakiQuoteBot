// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

using Microsoft.Bot.Schema;
using Jurumani.BotBuilder.Utils;

using Microsoft.Bot.Builder.Dialogs;
using Jurumani.BotBuilder.Dialogs;
using Jurumani.BotBuilder.Models;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot: ActivityHandler 
    {
        private static readonly MemoryStorage _myStorage = new MemoryStorage();
         private DialogSet _dialogs;
        private List<CameraModel> _cameramodels;
      
        private List<ProductModel> _switchmodels;
        private static string ADDPRODUCTSENDPOINT = "crm.quote.productrows.set";
        private static string DEALENDPOINT = "crm.deal.add";
        private static string QUOTEENDPOINT = "crm.quote.add";
        private static string TERMS = "Quotation valid for 7 days , and subject to an exchange current rand rate";
        private readonly QuoteBotAccessors _botAccessors;
        private readonly IConfiguration Configuration;
        private ProductModel wifiProduct;

        public EchoBot(ConversationState paramConvoState, IConfiguration configuration)
        {
            if (paramConvoState == null)
            {
                throw new System.ArgumentNullException(nameof(
                   paramConvoState));
            }
            Configuration = configuration;
           

            _botAccessors = new QuoteBotAccessors(paramConvoState)
            {
                ConversationDialogState = paramConvoState.CreateProperty<DialogState>("ConversationState"),
                UserProfile = paramConvoState.CreateProperty<CustomerModel>("CustomerProfile"),
                QuoteBasket = paramConvoState.CreateProperty<QuoteBasketModel>("QuoteBasket")
            };

            _dialogs = new DialogSet(_botAccessors.ConversationDialogState);

            //_dialogs.Add((new WaterfallDialog("QuoteDialog", new WaterfallStep[] { IntroStepAsync })));
            _dialogs.Add(new WaterfallDialog("CustomerProfileDialog", new WaterfallStep[]{
            LicenseDurationStepAsync,
             PhoneStepAsync,
             NameStepAsync,
             LastNameStepAsync,
             EmailStepAsync,
             CompanyStepAsync,
             JumpTopDeviceOptionsStepAsync

          }));
            _dialogs.Add(new TextPrompt(nameof(TextPrompt)));
            _dialogs.Add(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
            _dialogs.Add(new ChoicePrompt(nameof(ChoicePrompt)));
            _dialogs.Add
                 (new TextPrompt(nameof(TextPrompt)));
            _dialogs.Add(new ConfirmPrompt(nameof(ConfirmPrompt)));
            _dialogs.Add(new WaterfallDialog("WifiDialog", new WaterfallStep[] {
                 ChooseWifiLicenseType,
              ModelStepAsync,
               QuantityStepAsync,
               SummaryStepAsync,

            }));
            _dialogs.Add(new WaterfallDialog("SwitchDialog", new WaterfallStep[] {

              SwitchModelStepAsync,
               SwitchQuantityStepAsync,
               SwitchSummaryStepAsync,
            }));
            _dialogs.Add(new WaterfallDialog("FirewallDialog", new WaterfallStep[] {
                firewallLicenseTypeStepAsync,
                FirewallModelStepAsync,
                FirewallQuantityStepAsync,
                FirewallSummaryStepAsync
            }));
            _dialogs.Add(new WaterfallDialog("CameraDialog",new WaterfallStep[] {
                CameraModelStepAsync,
                CameraQuantityStepAsync,
                CameraSummaryStepAsync,
            }));
            _dialogs.Add(new WaterfallDialog("AddMoreDevicesDialog", new WaterfallStep[] {

            EndOrRestartStepAsync,
             SaveOrContinueStepAsync
            }));
            _dialogs.Add(new WaterfallDialog("DeviceBOMDialog", new WaterfallStep[]
            {
            DeviceOptionsListAsync,
            DeviceSpecsStepAsync,


             }));
            _dialogs.Add(new WaterfallDialog("IntroDialog",new WaterfallStep[]
            {
                GreetingStepAsync,
               
            }));
            _dialogs.Add(new ChoicePrompt("ContinueorEnd"));



        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationtoken = default(CancellationToken)) {
            await base.OnTurnAsync(turnContext, cancellationtoken);
          
            //save any state changes that might have occurred during the turn
           
            if (turnContext.Activity.Type == ActivityTypes.Message) {
               
                 
                if (!turnContext.Responded)
                {
                    var dialogContext =await _dialogs.CreateContextAsync(turnContext, cancellationtoken);
                    _botAccessors.ConversationState.CreateProperty<CustomerModel>("CustomerProfile");
                    _botAccessors.ConversationState.CreateProperty<QuoteBasketModel>("QuoteBasket");
                    await dialogContext.BeginDialogAsync("IntroDialog", null, cancellationtoken);
                }

            }
            else if (turnContext.Activity.Type == ActivityTypes.Event)
            {

            //    if (turnContext.Activity.Type == "webchat/join")
            //    {
            //        var dialogContext =await _dialogs.CreateContextAsync(turnContext, cancellationtoken);
            //        await turnContext.SendActivitiesAsync(new Activity[] {
            //    new Activity { Type = ActivityTypes.Typing },
            //    new Activity { Type = "delay", Value= 5000 },
                
            //},
            //cancellationtoken);
            //        var res = turnContext.Activity.CreateReply();
            //        var card = new HeroCard()
            //        {
            //            Title = $"Hi, I'm MeriQ. ",
            //            Subtitle = "Welcome to Jurumani Cloud Solutions sales division.",
            //            Text = $"  I can help you make quotes. You can start by choosing Create Quote,or type 'quote' or End to stop chatting",

            //        };
            //        res.Attachments = new List<Attachment>() { card.ToAttachment() };

            //        await turnContext.SendActivityAsync(res, cancellationtoken);
            //        await dialogContext.BeginDialogAsync("IntroDialog", null, cancellationtoken);
            //    }
            //}
            //{fae

            }


                await _botAccessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationtoken);


        }


        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            
            var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
            if (turnContext.Activity.Text.ToLowerInvariant() == "end conversation")
            {
                await turnContext.SendActivitiesAsync(
                    new Activity[]
                    {
                        MessageFactory.Text("Alright ending conversation..."),
                    }, cancellationToken
                    );
              await _botAccessors.UserProfile.SetAsync(turnContext, new CustomerModel(), cancellationToken);
                await _botAccessors.QuoteBasket.SetAsync(turnContext, new QuoteBasketModel(), cancellationToken);
                await dialogContext.BeginDialogAsync("IntroDialog");

            } else if (turnContext.Activity.Text.ToLowerInvariant() == "create quote")
            {
               
                await dialogContext.BeginDialogAsync("CustomerProfileDialog",null,cancellationToken);
            }
             

            else
            {
           
                await dialogContext.ContinueDialogAsync(cancellationToken);
            }



        }
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {

            var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);

            foreach (var member in membersAdded)
            {
                if (member.Id == turnContext.Activity.Recipient.Id)
                {

                    _botAccessors.ConversationState.CreateProperty<QuoteBasketModel>("QuoteBasket");
                    await dialogContext.BeginDialogAsync("IntroDialog", null, cancellationToken);
                };




            }
        }


        private async Task<DialogTurnResult> StartQuoteAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.ReplaceDialogAsync("CustomerProfileDialog");
        }




        #region CUSTOMERPROFILE STEPS
        private async Task<DialogTurnResult> LicenseDurationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationtoken)
        {
            var response = MessageFactory.Text("Please choose the duration for your license");
            response.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                  {
                      new CardAction()
                      {
                         Title="1Y",
                         Value="1",
                         Type=ActionTypes.ImBack,
                      },
                    new CardAction()
                      {
                         Title="3Y",
                         Value="3",
                         Type=ActionTypes.ImBack,
                      },
                    new CardAction()
                      {
                         Title="5Y",
                         Value="5",
                         Type=ActionTypes.ImBack,
                      }
                  }
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = response }, cancellationtoken);
        }

        private async Task<DialogTurnResult> PhoneStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Context.Activity.ChannelId == "telegram")
            {

            }
            var _licenseDuration = stepContext.Result.ToString();
            
            var _basket = await _botAccessors.QuoteBasket.GetAsync(stepContext.Context, () => new QuoteBasketModel());
            _basket.licenseDuration = _licenseDuration;
            await _botAccessors.QuoteBasket.SetAsync(stepContext.Context, _basket, cancellationToken);
            stepContext.Values["customerinfo"] = new CustomerModel();
            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
            {
                Prompt = MessageFactory.Text
                ("Please enter your Phone Number")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {

            var customerProfile = (CustomerModel)stepContext.Values["customerinfo"];
            var customerDetails = CustomerUtil.getCustomerByPhoneNumber(stepContext.Result.ToString());
            if (customerDetails.Count > 0)
            {
                customerProfile = customerDetails[0];
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Hi {customerDetails[0].NAME} {customerDetails[0].LAST_NAME}  company id is {customerDetails[0].COMPANY_ID}"));
                customerProfile.NAME = customerDetails[0].NAME;

                var Basket = await _botAccessors.QuoteBasket.GetAsync(stepContext.Context, () => new QuoteBasketModel());
                Basket.customer = customerProfile;

                await _botAccessors.QuoteBasket.SetAsync(stepContext.Context, Basket, cancellation);
             
                return await stepContext.ReplaceDialogAsync("DeviceBOMDialog");
            }
            else
            {

                var phonegeneric = new Generics();
                phonegeneric.VALUE = stepContext.Result.ToString();
                List<Generics> PHONE = new List<Generics>() { phonegeneric };

                var custdata = (CustomerModel)stepContext.Values["customerinfo"];
                custdata.PHONE = PHONE;
                //var cacheCustomerProfile = await _botAccessors.QuoteBasket.GetAsync(stepContext.Context, () => new QuoteBasketModel(), cancellation);
                //cacheCustomerProfile.customer = custdata;
                //await _botAccessors.QuoteBasket.SetAsync(stepContext.Context, cacheCustomerProfile, cancellation);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter your Name") }, cancellation);
            }

        }
        private async Task<DialogTurnResult> LastNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            var customerdata = (CustomerModel)stepContext.Values["customerinfo"];
            if (stepContext.Result.ToString() != null)
            {
                customerdata.NAME = stepContext.Result.ToString();

            }

            //var cacheCustomerProfile = await _botAccessors.UserProfile.GetAsync(stepContext.Context, () => new CustomerModel(), cancellation);
            //cacheCustomerProfile = customerdata;
            //await _botAccessors.UserProfile.SetAsync(stepContext.Context, cacheCustomerProfile, cancellation);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter your Surname") }, cancellation);
        }

        private async Task<DialogTurnResult> EmailStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            var customerdata = (CustomerModel)stepContext.Values["customerinfo"];
            customerdata.LAST_NAME = stepContext.Result.ToString();
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("What is your email address") }, cancellation);
        }

        private async Task<DialogTurnResult> CompanyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationtoken)
        {
            var customerdata = (CustomerModel)stepContext.Values["customerinfo"];
            var emailgeneric = new Generics();
            if (stepContext.Result.ToString() != null)
            {
                emailgeneric.VALUE = stepContext.Result.ToString();
            }

            List<Generics> EMAIL = new List<Generics>() { emailgeneric };
            customerdata.EMAIL = EMAIL;

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter your company name")
            });
        }

        private async Task<DialogTurnResult> JumpTopDeviceOptionsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationtoken)
        {
            var customerdata = (CustomerModel)stepContext.Values["customerinfo"];
            var company_name = "";
            if (stepContext.Result.ToString() != null)

            {
                company_name = stepContext.Result.ToString();
            }


            var companyID = CompanyUtil.SaveCompanyDetails(company_name);
            customerdata.COMPANY_ID = companyID;
            var customerID = CustomerUtil.saveCustomerInformation(customerdata.NAME, customerdata.LAST_NAME, customerdata.COMPANY_ID, customerdata.EMAIL[0].VALUE, customerdata.PHONE[0].VALUE);
            

            var Basket = await _botAccessors.QuoteBasket.GetAsync(stepContext.Context, () => new QuoteBasketModel());
            customerdata.ID = customerID;

            Basket.customer= customerdata;

            await _botAccessors.QuoteBasket.SetAsync(stepContext.Context, Basket, cancellationtoken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(customerdata.ToString()));


            return await stepContext.ReplaceDialogAsync("DeviceBOMDialog");

        }




        #endregion

        #region MERAKIBOMDIALOG STEPS

        private static List<CardAction> _Devices = new List<CardAction>() {
            new CardAction(){
                Title="Wifi ",
                Type=ActionTypes.ImBack,
                Value="Wifi" },
           new CardAction(){
               Title="Firewall ",
               Type=ActionTypes.ImBack,
               Value="Firewall"},
            new CardAction()
            {
                Title="Camera",
                Value="Camera",
                Type=ActionTypes.ImBack,

            },
             new CardAction()
            {
                Title="Switch",
                Value="Switch",
                Type=ActionTypes.ImBack
            }


       };

        private async Task<DialogTurnResult> DeviceOptionsListAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var Basket = await _botAccessors.QuoteBasket.GetAsync(stepContext.Context, (() => new QuoteBasketModel())); 
            // stepContext.Values["licenseduration"] = stepContext.Result.ToString();
            stepContext.Values["selecteddevice"] = "";
            var response = MessageFactory.Text("Please select a device Type");
            response.SuggestedActions = new SuggestedActions()
            {
                Actions = _Devices,
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = response }, cancellationToken);


        }
        private async Task<DialogTurnResult> DeviceSpecsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var result = (string)stepContext.Values["selecteddevice"];
            result = (string)stepContext.Result;
            switch (result)
            {
                case "Wifi":

                    return await stepContext.ReplaceDialogAsync("WifiDialog");

                case "Switch":
                    return await stepContext.ReplaceDialogAsync("SwitchDialog");

                case "Firewall":
                    return await stepContext.ReplaceDialogAsync("FirewallDialog");

                case "Camera":
                    return await stepContext.ReplaceDialogAsync("CameraDialog");


            }



            await stepContext.Context.SendActivityAsync(result);
            return await stepContext.ContinueDialogAsync(cancellationToken);
        }


        #endregion

        #region WIFIDIALOG STEPS
        private async Task<DialogTurnResult> ChooseWifiLicenseType(WaterfallStepContext stepContext, CancellationToken cancellationtoken)
        {
            var reply = MessageFactory.Text("Choose the License type for your wifi device");

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
        {
            new CardAction() { Title = "Advanced", Type = ActionTypes.ImBack, Value = "ADV" },
            new CardAction() { Title = "Enterprise", Type = ActionTypes.ImBack, Value = "ENT" },

        },
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = reply
            }, cancellationtoken);

        }

        private async Task<DialogTurnResult> ModelStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var _basket =await  _botAccessors.QuoteBasket.GetAsync(stepContext.Context, () => new QuoteBasketModel(), cancellationToken);
            var _licenseduration = _basket.licenseDuration;
            var _wifilicensetype = stepContext.Result.ToString();
            if (_wifilicensetype == "Advanced" || _wifilicensetype== "ADV")
            {
                _wifilicensetype = "ADV";
                stepContext.Values["wifilicense"] = $"LIC-MR-{_wifilicensetype}-{_licenseduration}";
            }
            else if (_wifilicensetype == "Enterprise" || _wifilicensetype == "ENT") {
                _wifilicensetype="ENT";
                stepContext.Values["wifilicense"] = $"LIC-{_wifilicensetype}-{_licenseduration}";
            }
           
            
           
            //await stepContext.Context.SendActivityAsync($"{stepContext.Values["wifilicense"]}");
            var _wifimodels= await WebServicesFactory.QueryProductData("MR", "LIC");
            var _models = ProductUtil.FilterforHardware(_wifimodels);
            var actionlist = new List<CardAction>() { };
            _models.ForEach(model =>
            {

                actionlist.Add(new CardAction
                {
                    Title = model.SKU,
                    Type = ActionTypes.ImBack,
                    Value = model.SKU

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
            var wifimodel = (string)stepContext.Result;
            stepContext.Values["wifimodel"] = wifimodel;

            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
            {
                Prompt = MessageFactory.Text($"How many {wifimodel} would you like?")
            });
        }
        private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var _wifiProduct = new BitrixProductRowModel();
            var _wifilicense = new BitrixProductRowModel();
            var Basket = await _botAccessors.QuoteBasket.GetAsync(stepContext.Context, () => new QuoteBasketModel(), cancellationToken);
            var _wifilicensesku = stepContext.Values["wifilicense"].ToString();
            var _wifimodel = (string)stepContext.Values["wifimodel"];
            var _wifiquantity = stepContext.Result.ToString();
            var _wifidata = await WebServicesFactory.QueryProductData(_wifimodel, "LIC");
            
            var _price = _wifidata.listJurumaniCloudInventory_Models.Items[0].LIST_PRICE_USD;
            //get the license for the  device
            var _strippedsku =  ProductUtil.generateSKU(_wifimodel);
            _strippedsku = _strippedsku += Basket.licenseDuration;
           
            var _licensedata = await WebServicesFactory.QueryProductData(_wifilicensesku, "HW");
            
            await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
            {
                Prompt = MessageFactory.Text($"You ordered {_wifiquantity}x {_wifimodel} at ${_price} each")
            }) ;
            _wifiProduct.PRICE = float.Parse(_price);
            _wifiProduct.PRODUCT_NAME = _wifimodel;
            _wifiProduct.QUANTITY = _wifiquantity;
            _wifilicense.PRICE = float.Parse(_licensedata.listJurumaniCloudInventory_Models.Items[0].LIST_PRICE_USD);
            _wifilicense.PRODUCT_NAME = _wifilicensesku;
            _wifilicense.QUANTITY = _wifiquantity;
            Basket.products.Add(_wifiProduct);
            Basket.products.Add(_wifilicense);
            

            await _botAccessors.QuoteBasket.SetAsync(stepContext.Context, Basket, cancellationToken);
            
            return await stepContext.ReplaceDialogAsync(nameof(AddMoreDevicesDialog), Basket);
        }


      

        #endregion

        #region ADDMOREDEVICESDIALOG
        private async Task<DialogTurnResult> EndOrRestartStepAsync
           (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var response = MessageFactory.Text("Would you like to add another device?");
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
        private async Task<DialogTurnResult> SaveOrContinueStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationtoken)
        {
            var answer = stepContext.Result.ToString().ToLowerInvariant();
            if (answer == "yes")
            {
                return await stepContext.ReplaceDialogAsync("DeviceBOMDialog");
            }
            else if(answer=="no")
            {
                //save all state
                var Basket = await _botAccessors.QuoteBasket.GetAsync(stepContext.Context, () => new QuoteBasketModel(), cancellationtoken);
              
                
                var customercompany = CompanyUtil.GetCompanyById(Basket.customer.COMPANY_ID);
               
                var dealid=DealsUtil.CreateDeal($"testdeal-{Basket.customer.NAME}", Basket.customer.COMPANY_ID, Basket.customer.COMPANY_ID, DEALENDPOINT);
               
                var quoteexpirationdate = DateTime.Today.AddDays(7);
                var quoteid = DealsUtil.CreateQuote(Basket.customer.NAME, Basket.customer.LAST_NAME, customercompany,dealid,Basket.customer.EMAIL[0].VALUE,quoteexpirationdate.ToString(),TERMS,QUOTEENDPOINT);
                var result = DealsUtil.AddProductsToQuote(quoteid, Basket.products, ADDPRODUCTSENDPOINT);
                if (result == "True")
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Your quote has been send"));
                }
                else
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Sorry, we could not process your request at the moment"));
                }
              
                Basket.clearProducts();
               
                return await stepContext.ReplaceDialogAsync("IntroDialog");
            }
            else
            {
                return await stepContext.EndDialogAsync(cancellationtoken);
            }
           
        }
        #endregion

        #region INTRODIALOGSTEPS
        private static async Task<DialogTurnResult> GreetingStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationtoken)
        {
            var username = "";
            if (stepContext.Context.Activity.ChannelId == "telegram")
            {
                 username= stepContext.Context.Activity.AsMessageActivity().ChannelData.message.from.first_name;
            }
          
            var res = stepContext.Context.Activity.CreateReply();
            var card = new HeroCard()
            {
                Title = $"Hi {username}, I'm MeriQ. ",
                Subtitle = "Welcome to Jurumani Cloud Solutions sales division.",
                Text = $"  I can help you make quotes. You can start by choosing Create Quote,or type 'create quote' or 'Quit' to stop chatting",

            };
            res.Attachments = new List<Attachment>() { card.ToAttachment() };

            await stepContext.Context.SendActivityAsync(res, cancellationtoken);

            var response = MessageFactory.Text("Please Choose what you would like me to help you with:");
            response.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){Title="Create Quote",Type=ActionTypes.ImBack,Value="create quote" },
                    new CardAction(){Title="Quit",Type=ActionTypes.ImBack,Value="quit"}
                },
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = response }, cancellationtoken);

        }
       
        
        #endregion

        #region CAMERADIALOG
        private async Task<DialogTurnResult> CameraModelStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
           
            var _cameramodels = await WebServicesFactory.QueryProductData("MV", "LIC");
            var _models = ProductUtil.FilterforHardware(_cameramodels);
            var actionlist = new List<CardAction>() { };
            _models.ForEach(model =>
            {

                actionlist.Add(new CardAction
                {
                    Title = model.SKU,
                    Type = ActionTypes.ImBack,
                    Value = model.SKU

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

        private async Task<DialogTurnResult> CameraQuantityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
           
            var cameramodel = (string)stepContext.Result;
            stepContext.Values["cameramodel"] = cameramodel;

            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
            {
                Prompt = MessageFactory.Text($"How many {cameramodel} would you like?")
            });
        }
        private async Task<DialogTurnResult> CameraSummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var _Basket = await _botAccessors.QuoteBasket.GetAsync(stepContext.Context, () => new QuoteBasketModel(), cancellationToken);
            var _licenseduration = _Basket.licenseDuration;
            var _licensesku = $"LIC-MV-{_licenseduration}";
            var _cameramodel = new BitrixProductRowModel();
            var _cameralicense = new BitrixProductRowModel();
            var _title = (string)stepContext.Values["cameramodel"];
            var _quantity= float.Parse(stepContext.Result.ToString());
            _cameramodel.PRODUCT_NAME = _title;
            _cameramodel.QUANTITY = _quantity.ToString();
            var _cameraresponse= await WebServicesFactory.QueryProductData(_title,"LIC");
            var _cameralicensedata = await WebServicesFactory.QueryProductData(_licensesku, "HW");
                        _cameramodel.PRICE=float.Parse(_cameraresponse.listJurumaniCloudInventory_Models.Items[0].LIST_PRICE_USD);
            _cameralicense.PRODUCT_NAME = _licensesku;
            _cameralicense.PRICE = float.Parse(_cameralicensedata.listJurumaniCloudInventory_Models.Items[0].LIST_PRICE_USD);
            _cameralicense.QUANTITY = _quantity.ToString();
            await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
            {
                Prompt = MessageFactory.Text($"You ordered {stepContext.Result}x {stepContext.Values["cameramodel"]}  at {_cameramodel.PRICE} each")
            });
            _Basket.products.Add(_cameramodel);
            _Basket.products.Add(_cameralicense);
            await _botAccessors.QuoteBasket.SetAsync(stepContext.Context, _Basket, cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(_Basket.ToString()));
            return await stepContext.ReplaceDialogAsync("AddMoreDevicesDialog");
        }

        #endregion

        #region FIREWALL STEPS
        private async Task<DialogTurnResult> firewallLicenseTypeStepAsync(WaterfallStepContext stepContext,CancellationToken cancellationtoken)
        {
            var actionlist = new List<CardAction>() { };
          

                actionlist.Add(new CardAction
                {
                    Title = "Advanced Security",

                    Type = ActionTypes.ImBack,
                    Value = "SEC"

                });

            actionlist.Add(new CardAction
            {
                Title="Enterprise",
                Type=ActionTypes.ImBack,
                Value="ENT"
               
            });
        

            var response = MessageFactory.Text($"Please choose a license for your firewall");
            response.SuggestedActions = new SuggestedActions()
            {
                Actions = actionlist


            };

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = response


            });
        }
        

        
        private async Task<DialogTurnResult> FirewallModelStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var _firewalllicensetype = stepContext.Result.ToString();
            if (_firewalllicensetype == "Enterprise")
            {
                stepContext.Values["firewalllicensetype"] = "ENT";
            }
            else
            {
                stepContext.Values["firewalllicensetype"] = "SEC";
            }


          
            var  _firewallmodels = await WebServicesFactory.QueryProductData("MX", "LIC");
             //filter for only hardware SKU
            var actionlist = new List<CardAction>() { };
            var _models = ProductUtil.FilterforHardware(_firewallmodels);
            _models.ForEach(model =>
            {

                actionlist.Add(new CardAction
                {
                    Title = model.SKU,

                    Type = ActionTypes.ImBack,
                    Value = model.SKU

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
            var _Basket = await _botAccessors.QuoteBasket.GetAsync(stepContext.Context, () => new QuoteBasketModel(), cancellationToken);
            var _licensduration = _Basket.licenseDuration;
            var _firewalllicensetype = stepContext.Values["firewalllicensetype"].ToString();
            var _firewallmodel = new  BitrixProductRowModel();
            var _firewalllicense = new BitrixProductRowModel();
               var _title = (string)stepContext.Values["firewallmodel"];
            var _quantity = stepContext.Result.ToString();
            //add function to request the price of the model
            _title = ProductUtil.generateSKU(_title);
            var _licensesku = $"LIC-{_title}-{_firewalllicensetype}-{_licensduration}";
           // await stepContext.Context.SendActivityAsync(MessageFactory.Text(_licensesku));
            var _firewalldata =await  WebServicesFactory.QueryProductData(_title, "LIC");
            var _firewalllicensedata = await WebServicesFactory.QueryProductData(_licensesku, "HW");
            if (_firewalldata.listJurumaniCloudInventory_Models.Items.Count == 0)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Product {_title} was not found"));
            }
            else
            {
                var _price = _firewalldata.listJurumaniCloudInventory_Models.Items[0].LIST_PRICE_USD;
                await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
                {
                    Prompt = MessageFactory.Text($"You ordered {_quantity}x {_title}  at ${_price} each")
                });
                _firewallmodel.PRICE = float.Parse(_price);
                _firewallmodel.QUANTITY = _quantity;
                _firewallmodel.PRODUCT_NAME = _title;
                _firewalllicense.PRICE = float.Parse(_firewalllicensedata.listJurumaniCloudInventory_Models.Items[0].LIST_PRICE_USD);
                _firewalllicense.PRODUCT_NAME = _firewalllicensedata.listJurumaniCloudInventory_Models.Items[0].SKU;
                _firewalllicense.QUANTITY = _quantity;

                
                _Basket.products.Add(_firewallmodel);
                _Basket.products.Add(_firewalllicense);
                await _botAccessors.QuoteBasket.SetAsync(stepContext.Context, _Basket, cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(_Basket.ToString()));
                
            }
            return await stepContext.ReplaceDialogAsync("AddMoreDevicesDialog");

        }

        #endregion

        #region SWITCHSTEPS
        private async Task<DialogTurnResult> SwitchModelStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var _switchmodels = await WebServicesFactory.QueryProductData("MS", "LIC");

            var actionlist = new List<CardAction>() { };
            _switchmodels.listJurumaniCloudInventory_Models.Items.ForEach(model =>
            {

                actionlist.Add(new CardAction
                {
                    Title = model.SKU,
                    Type = ActionTypes.ImBack,
                    Value = model.SKU

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
            var _switchlicense = new BitrixProductRowModel();
            var _Basket= await _botAccessors.QuoteBasket.GetAsync(stepContext.Context, () => new QuoteBasketModel(), cancellationToken);
            var _licenseduration = _Basket.licenseDuration;
            var _title = stepContext.Values["switchmodel"].ToString();
       
            var _strippedsku = ProductUtil.generateSKU(_title);
             var _quantity = stepContext.Result.ToString();
            //return to menu if the sku wasnt found
           
            var _licenseparam = $"{_strippedsku}-{_licenseduration}";
            //await stepContext.Context.SendActivityAsync(_licenseparam);
            var _switchdata = await WebServicesFactory.QueryProductData(_title, "LIC");
            var _switchlicensedata = await WebServicesFactory.QueryProductData(_licenseparam,"HW");
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text("sku" + _switchlicensedata.listJurumaniCloudInventory_Models.Items[0].SKU));
            if (_switchdata.listJurumaniCloudInventory_Models.Items.Count == 0)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Product {_title} wasn't found in the catalog"));

            }
            else {
                var _price = float.Parse(_switchdata.listJurumaniCloudInventory_Models.Items[0].LIST_PRICE_USD);
                _switchmodel.PRICE = _price;
                _switchmodel.PRODUCT_NAME = _title;
                _switchmodel.QUANTITY = _quantity;
                _switchlicense.PRICE =float.Parse( _switchlicensedata.listJurumaniCloudInventory_Models.Items[0].LIST_PRICE_USD);
                _switchlicense.QUANTITY = _quantity;
                _switchlicense.PRODUCT_NAME = _switchlicensedata.listJurumaniCloudInventory_Models.Items[0].SKU;


                await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
                {
                    Prompt = MessageFactory.Text($"You ordered {stepContext.Result}x {stepContext.Values["switchmodel"]}s at ${_price} each")
                });
                
                _Basket.products.Add(_switchmodel);
                _Basket.products.Add(_switchlicense);
                await _botAccessors.QuoteBasket.SetAsync(stepContext.Context, _Basket, cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(_Basket.ToString()));
            }


           
            return await stepContext.ReplaceDialogAsync("AddMoreDevicesDialog");
        }

        #endregion

        #region WIFISTEPS
 


        private async Task<DialogTurnResult> WifiQuantityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var wifimodel = stepContext.Result.ToString();
            stepContext.Values["wifimodel"] = wifimodel;

            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
            {
                Prompt = MessageFactory.Text($"How many {wifimodel} would you like?")
            });
        }
        private async Task<DialogTurnResult> WifiSummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var Basket = await _botAccessors.QuoteBasket.GetAsync(stepContext.Context, () => new QuoteBasketModel(), cancellationToken);
            var _wifiProduct = new BitrixProductRowModel();
            var _title = stepContext.Values["wifimodel"].ToString();
            var _quantity = stepContext.Result.ToString();
            var _wifidata = await WebServicesFactory.QueryProductData(_title, "LIC");
            if (_wifidata.listJurumaniCloudInventory_Models.Items.Count == 0)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Product {_title} was not found in our catalog"));

            }
            else
            {
                var _price = _wifidata.listJurumaniCloudInventory_Models.Items[0].LIST_PRICE_USD;
                _wifiProduct.PRODUCT_NAME = _title;
                _wifiProduct.QUANTITY = _quantity;
                _wifiProduct.PRICE = float.Parse(_price);
                await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
                {
                    Prompt = MessageFactory.Text($"You ordered {stepContext.Result}x {stepContext.Values["wifimodel"] } at {_price} each ")
                });
               
                Basket.products.Add(_wifiProduct);
                var convostate = _botAccessors.ConversationState.LoadAsync(stepContext.Context, false, cancellationToken);

                await _botAccessors.QuoteBasket.SetAsync(stepContext.Context, Basket, cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(Basket.ToString()));
            }

            
            return await stepContext.ReplaceDialogAsync("AddMoreDevicesDialog"
                );
        }

        #endregion

    }
}
