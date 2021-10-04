namespace AspNetCoreDemoApp.Providers.Models
{
    public record RunTaskModel(string Task, object? Payload, string WorkflowInstanceId);
}