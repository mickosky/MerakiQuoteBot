// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Jurumani.BotBuilder.Dialogs;
using Microsoft.BotBuilderSamples.Bots;
using Microsoft.Bot.Builder.EchoBot;

namespace Microsoft.BotBuilderSamples
{
    public class Startup
    {
        private const string BotOpenIdMetadataKey = "BotOpenIdMetadata";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //storage layer for keeping the user and conversation states
            var storage = new MemoryStorage();

            // Create the Bot Framework Adapter.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
            services.AddSingleton<IStorage, MemoryStorage>();
            services.AddSingleton<IntroDialog>();
            var userState = new UserState(storage);
            services.AddSingleton(userState);
            var conversationState = new ConversationState(storage);
            services.AddSingleton(conversationState);
            
            //services.AddSingleton<CustomerProfileDialog>();
           // services.AddSingleton<MerakiDeviceBoMDialog>();
            //services.AddSingleton<AddMoreDevicesDialog>();
            //services.AddSingleton<WifiDialog>();
            //services.AddSingleton<IntroDialog>();
            //services.AddSingleton<FirewallDialog>();
            //services.AddSingleton<SwitchDialog>();
           
            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot. 
            services.AddTransient<IBot, EchoBot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
           app.UseWebSockets();

            app.UseMvc();
        }
    }
}
