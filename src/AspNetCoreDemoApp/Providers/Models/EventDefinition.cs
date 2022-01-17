using System.Collections.Generic;
using Elsa.Models;

namespace AspNetCoreDemoApp.Providers.Models
{
    public class EventDefinition : Entity
    {
        public string Name { get; set; } = default!;
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public ICollection<string>? Outcomes { get; set; }
        public ICollection<TransitionModel>? Transitions { get; set; }
    }

    public class TransitionModel
    {
        public string Name { get; set; } = (string) null;

        public string? Syntax { get; set; } = "Liquid";

        public string Expression  { get; set; }
    }
}