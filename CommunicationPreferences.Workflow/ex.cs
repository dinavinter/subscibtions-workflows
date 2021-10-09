using Elsa.Options;
using Microsoft.Extensions.DependencyInjection;

namespace CommunicationPreferences.Workflow
{
    public static class ex
    {
        public static void AddCommunicationPreferences(this ElsaOptionsBuilder elsaOptionsBuilder)
        {
            elsaOptionsBuilder.AddActivity<CommunicationPage>()

                .AddWorkflowsFrom<CommunicationPreferencesWorkflow>();


        }
    }
}