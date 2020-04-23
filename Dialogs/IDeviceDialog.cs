
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace Jurumani.BotBuilder.Dialogs
{
    public interface IDeviceDialogs
    {
        public Task<DialogTurnResult> ModelStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken);
        public Task<DialogTurnResult> QuantityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken);
        public Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken);
    }
}
