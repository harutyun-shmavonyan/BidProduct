using BidProduct.API.Middlewares;
using BidProduct.Common.Exceptions;

namespace BidProduct.API.ExceptionHandlers
{
    public class FirstExceptionHandler : ExceptionHandlerBase
    {
        public FirstExceptionHandler(IEnumerable<ExceptionHandlerBase> exceptionHandlers)
        {
            var handlersList = exceptionHandlers.ToList();

            SetSuccessor(handlersList[0]);
            ExceptionHandlerBase lastExceptionHandler = null;
            for (var i = 0; i < handlersList.Count - 1; i++)
            {
                if (handlersList[i] is LastExceptionHandler)
                {
                    lastExceptionHandler = handlersList[i];
                    continue;
                }
                handlersList[i].SetSuccessor(handlersList[i + 1]);
            }

            handlersList[^1] = lastExceptionHandler;
        }

        public override ExceptionResult Execute(BidProductException exception) =>
            Next.Execute(exception);
    }
}