using System;
using System.Collections.Generic;
using System.Linq;

using Core;

using Newtonsoft.Json;

namespace KenticoKontent.Models.Management
{
    public class APIErrorResponse
    {
        public string? Message { get; set; }

        [JsonProperty("validation_errors")]
        public IEnumerable<ValidationError>? ValidationErrors { get; set; }

        public Exception GetException()
        {
            var message = Message;

            if (ValidationErrors != default && ValidationErrors.Any())
            {
                message += string.Join(", ", ValidationErrors.Select(error => $"{error.Path}: {error.Message}"));
            }

            return new ApiException(message);
        }
    }

    public class ValidationError
    {
        public string? Message { get; set; }

        public string? Path { get; set; }
    }
}