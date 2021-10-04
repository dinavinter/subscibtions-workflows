using Elsa.Models;

namespace AspNetCoreDemoApp.Providers.Models
{
    public class CommandDefinition : Entity
    {
        public string Name { get; set; } = default!;
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
    }
}