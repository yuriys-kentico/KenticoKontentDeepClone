using System.Collections.Generic;

using KenticoKontent.Models.Management.References;

namespace KenticoKontent.Models.Management.Elements
{
    public class AbstractReferenceListElement : AbstractElement
    {
        public IList<Reference>? Value { get; set; }
    }
}