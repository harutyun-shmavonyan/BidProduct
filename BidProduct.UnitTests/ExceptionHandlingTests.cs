using System.Collections.Generic;
using System.Net;
using BidProduct.API.ExceptionHandlers;
using BidProduct.Common.Exceptions;
using BidProduct.UnitTests.Abstract;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BidProduct.UnitTests
{
    [TestFixture]
    public class ExceptionHandlingTests : TestBase
    {
        private string ExceptionMessage = "Exception message";

        [Test]
        public void DbUpdateExceptionHandlerTest_ShouldHandle()
        {
            //Arrange
            var exception = new BidProductException(ExceptionMessage, ExceptionType.DbUpdateFailed);
            var handler = new DbUpdateExceptionHandler();

            //Act
            var result = handler.Execute(exception);

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Be(ExceptionMessage);
        }

        [Test]
        public void InvalidOperationExceptionHandlerTest_ShouldHandle()
        {
            //Arrange
            var exception = new BidProductException(ExceptionMessage, ExceptionType.InvalidOperation);
            var handler = new InvalidOperationExceptionHandler();

            //Act
            var result = handler.Execute(exception);

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Be(ExceptionMessage);
        }

        [Test]
        public void ValidationFailedExceptionHandlerTest_ShouldHandle()
        {
            //Arrange
            var exception = new BidProductException(ExceptionMessage, ExceptionType.ValidationFailed);
            var handler = new ValidationFailedExceptionHandler();

            //Act
            var result = handler.Execute(exception);

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Message.Should().Be(ExceptionMessage);
        }

        [Test]
        public void NotFoundExceptionHandlerTest_ShouldHandle()
        {
            //Arrange
            var exception = new BidProductException(ExceptionMessage, ExceptionType.NotFound);
            var handler = new NotFoundExceptionHandler();

            //Act
            var result = handler.Execute(exception);

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Message.Should().Be(ExceptionMessage);
        }

        [Test]
        public void ExceptionHandlerBase_SetNextShouldWork()
        {
            //Arrange
            var firstExceptionHandler = new FirstExceptionHandler(new List<ExceptionHandlerBase> { new DbUpdateExceptionHandler() });
            var lastExceptionHandlerMock = new Mock<LastExceptionHandler>();
            lastExceptionHandlerMock.Setup(x => x.Execute(It.IsAny<BidProductException>()));

            //Act
            firstExceptionHandler.SetSuccessor(lastExceptionHandlerMock.Object);
            firstExceptionHandler.Execute(new BidProductException(string.Empty, ExceptionType.NotFound));

            //Assert
            lastExceptionHandlerMock.Verify(h => h.Execute(It.IsAny<BidProductException>()));
        }

        [Test]
        public void ExceptionHandlerBase_SetNextChainShouldWork()
        {
            //Arrange
            var firstExceptionHandler = new FirstExceptionHandler(new List<ExceptionHandlerBase> { new DbUpdateExceptionHandler(), new DbUpdateExceptionHandler() });
            var lastExceptionHandlerMock = new Mock<LastExceptionHandler>();
            lastExceptionHandlerMock.Setup(x => x.Execute(It.IsAny<BidProductException>()));

            //Act
            firstExceptionHandler.SetSuccessor(lastExceptionHandlerMock.Object);
            firstExceptionHandler.Execute(new BidProductException(string.Empty, ExceptionType.NotFound));

            //Assert
            lastExceptionHandlerMock.Verify(h => h.Execute(It.IsAny<BidProductException>()));
        }

        [Test]
        public void LastExceptionHandler_ShouldWork()
        {
            //Arrange
            ServiceCollection.AddTransient<FirstExceptionHandler>();
            ServiceCollection.AddTransient<ExceptionHandlerBase, DbUpdateExceptionHandler>();
            ServiceCollection.AddTransient<ExceptionHandlerBase, ValidationFailedExceptionHandler>();
            ServiceCollection.AddTransient<ExceptionHandlerBase, InvalidOperationExceptionHandler>();
            ServiceCollection.AddTransient<ExceptionHandlerBase, NotFoundExceptionHandler>();
            ServiceCollection.AddTransient<ExceptionHandlerBase, LastExceptionHandler>();

            var firstExceptionHandler = ServiceProvider.GetRequiredService<FirstExceptionHandler>();

            //Act
            var result = firstExceptionHandler.Execute(new BidProductException(string.Empty, ExceptionType.Default));

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Be($"{ExceptionType.Default} wasn't handled");
        }

        [Test]
        public void ExceptionHandlerBase_Execute()
        {
            //Arrange
            var testExceptionHandler = new TestExceptionHandler();

            //Act
            var result = testExceptionHandler.Execute(new BidProductException(ExceptionMessage, default));

            //Assert
            result.StatusCode.Should().Be(default);
            result.Message.Should().Be(ExceptionMessage);
        }
    }
}
