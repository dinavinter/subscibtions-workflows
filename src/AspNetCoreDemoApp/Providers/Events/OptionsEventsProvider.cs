using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Activities.Conductor.Models;
using Elsa.Activities.Conductor.Options;
using Microsoft.Extensions.Options;
using EventDefinition = AspNetCoreDemoApp.Providers.Models.EventDefinition;

namespace AspNetCoreDemoApp.Providers.Events
{
    public interface IEventsProvider
    {
        ValueTask<IEnumerable<EventDefinition>> GetEventsAsync(
            CancellationToken cancellationToken);
    }
    public class OptionsEventsProvider : IEventsProvider
    {
        private readonly StateOptions _options;
        public OptionsEventsProvider(IOptions<StateOptions> options) => _options = options.Value;
        public ValueTask<IEnumerable<EventDefinition>> GetEventsAsync(CancellationToken cancellationToken) => new(_options.Events);
    }


}