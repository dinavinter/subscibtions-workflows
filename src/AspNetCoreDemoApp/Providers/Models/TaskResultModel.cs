using System.Collections.Generic;

namespace AspNetCoreDemoApp.Providers.Models
{
    public record TaskResultModel(string TaskName, ICollection<string>? Outcomes, object? Payload, string? WorkflowInstanceId, string? CorrelationId);
}