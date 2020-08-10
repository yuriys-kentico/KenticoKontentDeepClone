using System.Collections.Generic;

namespace KenticoKontent.Models.Management.Elements
{
    public class RichTextElement : AbstractElement
    {
        public string? Value { get; set; }

        public IEnumerable<Component>? Components { get; set; }
    }
}