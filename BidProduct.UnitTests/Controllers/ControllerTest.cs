using System.Threading.Tasks;
using BidProduct.Common.Abstract;
using BidProduct.SL.Abstract;
using BidProduct.SL.Abstract.CQRS;
using BidProduct.UnitTests.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BidProduct.UnitTests.Controllers
{
    public abstract class ControllerTest : TestBase
    {
        protected IMapper Mapper => ServiceProvider.GetRequiredService<IMapper>();
        protected Mock<IInternalMediator> MockMediator = new();
        protected IInternalMediator Mediator => MockMediator.Object;

        protected void AddMockResponse<TRequest, TResponse>(TResponse response) where TRequest : IInternalRequest<TResponse>
        {
            MockMediator.Setup(m => m.SendAsync(It.IsAny<TRequest>(), default)).Returns(Task.FromResult(response));
        }
    }
}
