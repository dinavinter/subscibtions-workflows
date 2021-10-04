namespace AspNetCoreDemoApp.Providers.Models
{
    public record SendCommandModel(string Command, object? Payload, string WorkflowInstanceId);
}