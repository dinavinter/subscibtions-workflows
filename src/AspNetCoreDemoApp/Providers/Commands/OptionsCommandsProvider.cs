using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Activities.Conductor.Models;
using Microsoft.Extensions.Options;
using CommandDefinition = AspNetCoreDemoApp.Providers.Models.CommandDefinition;

namespace AspNetCoreDemoApp.Providers.Commands
{
    public class OptionsCommandsProvider : ICommandsProvider
    {
        private readonly StateOptions _options;
        public OptionsCommandsProvider(IOptions<StateOptions> options) => _options = options.Value;
        public ValueTask<IEnumerable<CommandDefinition>> GetCommandsAsync(CancellationToken cancellationToken) => new(_options.Actions);
    }

    public interface ICommandsProvider
    {
        ValueTask<IEnumerable<CommandDefinition>> GetCommandsAsync(
            CancellationToken cancellationToken);
    }
}