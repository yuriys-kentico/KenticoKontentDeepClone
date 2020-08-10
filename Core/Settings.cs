using System;

namespace Core
{
    public class Settings
    {
        public Guid ProjectId { get; set; }

        public string ManagementApiKey { get; set; } = string.Empty;
    }
}