// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Net.Http;

namespace Microsoft.Bot.Builder.EchoBot
{
    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        public AdapterWithErrorHandler(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger)
            : base(configuration, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");
                if (exception.InnerException is HttpRequestException)
                {
                    await turnContext.SendActivityAsync(":( You seem to be facing a network issue,please check your  internet connection and restart the conversation");

                }
                else
                {

                    // Send a message to the user
                    await turnContext.SendActivityAsync("We apologize,something went went wrong");
                    await turnContext.SendActivityAsync("Our Dev team has been notified and this will be rectified");

                    // Send a trace activity, which will be displayed in the Bot Framework Emulator
                    await turnContext.TraceActivityAsync("OnTurnError Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError");
                }
            };
        }
    }
}