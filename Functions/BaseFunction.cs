using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Functions
{
    public class BaseFunction
    {
        private readonly ILogger logger;

        protected const string transfers = nameof(transfers);

        protected BaseFunction(ILogger logger)
        {
            this.logger = logger;
        }

        protected IActionResult LogOk()
        {
            logger.LogInformation("Ok");

            return new OkResult();
        }

        protected IActionResult LogOkObject(object? response)
        {
            logger.LogInformation("Ok object: {response}", response);

            return new OkObjectResult(response);
        }

        protected IActionResult LogUnauthorized()
        {
            logger.LogWarning("Unauthorized");

            return new NotFoundResult();
        }

        protected IActionResult LogException(Exception exception)
        {
            var message = exception.Message;

            logger.LogError(exception, "Error request: {message}", message);

            return new InternalServerErrorMessageResult(exception.Message);
        }

        protected IActionResult LogOkException(Exception exception)
        {
            var message = exception.Message;

            logger.LogError(exception, "Error request: {message}", message);

            return new OkObjectResult(message);
        }

        protected IActionResult LogBadRequest(Exception exception)
        {
            var message = exception.Message;

            logger.LogError(exception, "Error request: {message}", message);

            return new BadRequestObjectResult(exception.Message);
        }
    }
}