using BidProduct.API;
using BidProduct.Common.Abstract;
using BidProduct.SL.Abstract;
using BidProduct.SL.Abstract.CQRS;
using BidProduct.SL.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace BidProduct.UnitTests.ControllerTests
{
    public abstract class ControllerTest
    {
        protected IServiceCollection ServiceCollection;
        protected IConfiguration Configuration;

        protected IServiceProvider ServiceProvider => ServiceCollection.BuildServiceProvider();
        protected IMapper Mapper => ServiceProvider.GetRequiredService<IMapper>();
        protected Mock<IInternalMediator> MockMediator = new();
        protected IInternalMediator Mediator => MockMediator.Object;

        protected void AddMockResponse<TRequest, TResponse>(TResponse response) where TRequest : IInternalRequest<TResponse>
        {
            MockMediator.Setup(m => m.SendAsync(It.IsAny<TRequest>(), default)).Returns(Task.FromResult(response));
        }

        [SetUp]
        public void Initialize()
        {
            ServiceCollection = new ServiceCollection();
            Configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            ServiceCollection.AddAdditionalMapperProfile(new ViewModelsMappingProfile());
            ServiceCollection.AddApplicationServices(Configuration);
        }
    }
}
