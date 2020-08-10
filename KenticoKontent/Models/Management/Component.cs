using System;
using System.Collections.Generic;

using KenticoKontent.Models.Management.Elements;
using KenticoKontent.Models.Management.References;

namespace KenticoKontent.Models.Management
{
    public class Component
    {
        public Guid Id { get; set; }

        public Reference? Type { get; set; }

        public IList<AbstractElement> Elements { get; set; } = new List<AbstractElement>();
    }
}