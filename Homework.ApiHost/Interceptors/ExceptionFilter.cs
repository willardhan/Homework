using System;
using System.Net;
using Homework.Infrastructure.ComponentModels;
using Homework.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Homework.ApiHost.Interceptors
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(
            ILogger<ExceptionFilter> logger
            )
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);
            
            ContentResult result = new ContentResult
            {
                ContentType = "text/html;charset=utf-8"
            };

            if (context.Exception is OperationCanceledException)
            {
                result.Content = "operation canceled exception";
                result.StatusCode = (int) HttpStatusCode.InternalServerError;
            }
            else if (context.Exception is TimeoutException)
            {
                result.Content = "time out exception";
                result.StatusCode = (int) HttpStatusCode.InternalServerError;
            }
            else if (context.Exception is ValidationLevelException) // user define exception handler
            {
                result.Content = context.Exception.Message;
                result.StatusCode = (int) HttpStatusCode.InternalServerError;
            }
            else if (context.Exception is InputInvalidException) // user define exception handler
            {
                result.Content = GlobalParameter.InputInvalidExceptionMsg;
                result.StatusCode = (int) HttpStatusCode.InternalServerError;
            }
            else // un handler exception
            {
                result.Content = "system error";
                result.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            
            context.Result = result;
            context.ExceptionHandled = true;
        }
    }
}