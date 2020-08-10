using KenticoKontent.Models.Management.References;

namespace KenticoKontent.Models.Management.Types
{
    public class ElementType
    {
        public string? Id { get; set; }

        public string? Type { get; set; }

        public Reference? Snippet { get; set; }
    }
}