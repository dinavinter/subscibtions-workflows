using Elsa.Models;
using Elsa.Persistence;

namespace AspNetCoreDemoApp.DsStore
{
    public class DefinitionDsStore : DsStore<WorkflowDefinition>, IWorkflowDefinitionStore
    {
        public DefinitionDsStore(HttpJsonClient httpJsonClient) : base(httpJsonClient)
        {
        }
    }
}