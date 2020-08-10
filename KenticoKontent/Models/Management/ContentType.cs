using System.Collections.Generic;

using KenticoKontent.Models.Management.Types;

namespace KenticoKontent.Models.Management
{
    public class ContentType
    {
        public IList<ElementType>? Elements { get; set; }
    }
}