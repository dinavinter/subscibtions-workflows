using System.Collections.Generic;

namespace AspNetCoreDemoApp.Providers.Models
{
    public record EventModel(string EventName, ICollection<string>? Outcomes, object? Payload, string? WorkflowInstanceId, string? CorrelationId);
}