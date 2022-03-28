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
            for (var i = 0; i < handlersList.Count - 1; i++)
            {
                handlersList[i].SetSuccessor(handlersList[i + 1]);
            }
        }

        public override ExceptionResult Execute(BidProductException exception) =>
            Next.Execute(exception);
    }
}