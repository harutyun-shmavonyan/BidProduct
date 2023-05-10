using System.Text;
using BidProduct.API.ExceptionHandlers;
using BidProduct.Common.Exceptions;
using BidProduct.SL.Abstract;
using Newtonsoft.Json;

namespace BidProduct.API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly ExceptionHandlerBase _exceptionHandler;
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next, FirstExceptionHandler handler)
        {
            _next = next;
            _exceptionHandler = handler;
        }

        public async Task InvokeAsync(HttpContext context, ITraceIdProvider TraceIdProvider)
        {
            try
            {
                await _next(context);
            }
            catch (BidProductException e)
            {
                if (context.Response.HasStarted)
                {
                    throw;
                }

                var result = _exceptionHandler.Execute(e);
                result.TraceId = TraceIdProvider.ScopeGuid;

                context.Response.ContentType = ContentType.Json;
                context.Response.StatusCode = (int)result.StatusCode;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
            }
            catch (Exception e)
            {
                if (context.Response.HasStarted)
                {
                    throw;
                }

                context.Response.ContentType = ContentType.Json;
                context.Response.StatusCode = 500;

                var messageBuilder = new StringBuilder(e.Message);

                var innerException = e.InnerException;
                while (innerException != null)
                {
                    messageBuilder.Append(Environment.NewLine);
                    messageBuilder.Append(innerException.Message);

                    innerException = innerException.InnerException;
                }

                var error = new ExceptionResult
                {
                    Message = messageBuilder.ToString(),
                    Reason = "Internal Server Error, Stack Trace"
                };

                await context.Response.WriteAsync(JsonConvert.SerializeObject(error));
            }
        }
    }
}
