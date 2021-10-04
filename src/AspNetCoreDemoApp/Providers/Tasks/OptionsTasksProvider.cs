using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using AspNetCoreDemoApp.Providers.Models;

namespace AspNetCoreDemoApp.Providers.Tasks
{
    public class OptionsTasksProvider : ITasksProvider
    {
        private readonly StateOptions _options;
        public OptionsTasksProvider(IOptions<StateOptions> options) => _options = options.Value;
        public ValueTask<IEnumerable<TaskDefinition>> GetTasksAsync(CancellationToken cancellationToken) => new(_options.Services);
    }

    public interface ITasksProvider
    {
        ValueTask<IEnumerable<TaskDefinition>> GetTasksAsync(
            CancellationToken cancellationToken);
    }
}