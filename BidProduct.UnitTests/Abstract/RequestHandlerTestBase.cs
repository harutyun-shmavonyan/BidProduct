using System.Threading.Tasks;
using BidProduct.Common.Abstract;
using BidProduct.DAL.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BidProduct.UnitTests;

public abstract class RequestHandlerTestBase : TestBase
{
    protected IMapper Mapper => ServiceProvider.GetRequiredService<IMapper>();

    protected Mock<IUnitOfWork> UnitOfWorkMock
    {
        get
        {
            var mock = new Mock<IUnitOfWork>();
            mock.Setup(uo => uo.SaveAsync()).Returns(Task.CompletedTask);

            return mock;
        }
    }
}