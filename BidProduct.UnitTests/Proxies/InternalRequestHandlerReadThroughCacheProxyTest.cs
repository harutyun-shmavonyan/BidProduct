using BidProduct.DAL.Abstract.Cache;
using BidProduct.SL.Models.CQRS.Queries;
using BidProduct.SL.Models.CQRS.Responses;
using BidProduct.SL.Proxies.Cache;
using MediatR;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using BidProduct.DAL.Caches;
using BidProduct.UnitTests.Abstract;

namespace BidProduct.UnitTests.Proxies
{
    [TestFixture]
    public class InternalRequestHandlerReadThroughCacheProxyTest : TestBase
    {
        [Test]
        public async Task CacheExistsShouldReturnResult()
        {
            var handlerMock = new Mock<IRequestHandler<GetProductQuery, GetProductQueryResponse>>();
            var cacheMock = new Mock<IKeyValueCache<GetProductQuery, GetProductQueryResponse, long, GetProductQueryResponse>>();

            var response = new GetProductQueryResponse();
            cacheMock.Setup(c => c.GetAsync(It.IsAny<GetProductQuery>())).ReturnsAsync(response);

            var cacheProxy = new InternalRequestHandlerReadThroughCacheProxy<GetProductQuery, GetProductQueryResponse, long, GetProductQueryResponse>(
                handlerMock.Object, cacheMock.Object, new ExpirationPolicy<GetProductQuery, GetProductQueryResponse>(TimeSpan.MaxValue), new TestLogger());

            var result = await cacheProxy.HandleAsync(new GetProductQuery(), default);

            Assert.AreSame(result, response);
            handlerMock.Verify(h => h.Handle(It.IsAny<GetProductQuery>(), default), Times.Never);
        }

        [Test]
        public async Task CacheMissShouldCallHandleResult()
        {
            var handlerMock = new Mock<IRequestHandler<GetProductQuery, GetProductQueryResponse>>();
            var cacheMock = new Mock<IKeyValueCache<GetProductQuery, GetProductQueryResponse, long, GetProductQueryResponse>>();

            var response = new GetProductQueryResponse();
            cacheMock.Setup(c => c.GetAsync(It.IsAny<GetProductQuery>())).ReturnsAsync((GetProductQueryResponse)null!);
            handlerMock.Setup(h => h.Handle(It.IsAny<GetProductQuery>(), default)).ReturnsAsync(response);

            var cacheProxy = new InternalRequestHandlerReadThroughCacheProxy<GetProductQuery, GetProductQueryResponse, long, GetProductQueryResponse>
                (handlerMock.Object, cacheMock.Object, new ExpirationPolicy<GetProductQuery, GetProductQueryResponse>(TimeSpan.MaxValue), new TestLogger());
            
            var result = await cacheProxy.HandleAsync(new GetProductQuery(), default);

            Assert.AreSame(result, response);
            handlerMock.Verify(h => h.Handle(It.IsAny<GetProductQuery>(), default), Times.Once);
        }
    }
}
