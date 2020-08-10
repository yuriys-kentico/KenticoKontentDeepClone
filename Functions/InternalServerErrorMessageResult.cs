using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace Functions
{
    public class InternalServerErrorMessageResult : IActionResult
    {
        private readonly string message;

        public InternalServerErrorMessageResult(string message)
        {
            this.message = message;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await new StringContent(message).CopyToAsync(context.HttpContext.Response.Body);
        }
    }
}