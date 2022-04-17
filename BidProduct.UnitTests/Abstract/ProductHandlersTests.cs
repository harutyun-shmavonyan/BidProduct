using System;
using System.Threading.Tasks;
using BidProduct.DAL.Abstract;
using BidProduct.DAL.Models;
using BidProduct.SL.CQRS.QueryHandlers;
using BidProduct.SL.Models.CQRS.Commands;
using BidProduct.SL.Models.CQRS.Queries;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BidProduct.UnitTests.Abstract;

[TestFixture]
public class ProductHandlersTests : RequestHandlerTestBase
{
    [Test]
    public async Task GetProductQueryHander_ShouldReturn_GetProductQueryResponse()
    {
        //Arrange
        var id = 1;
        var created = DateTimeOffset.UtcNow;
        var name = "product name";

        var productRepositoryMock = new Mock<IProductRepository>();
        productRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .Returns(Task.FromResult(new Product
            {
                Id = id,
                Created = created,
                Name = name
            }));

        var handler = new GetProductQueryHandler(productRepositoryMock.Object, Mapper);

        //Act
        var response = await handler.HandleAsync(new GetProductQuery(id: id));

        //Arrange
        response.Id.Should().Be(id);
        response.Created.Should().Be(created);
        response.Name.Should().Be(name);
    }

    [Test]
    public async Task CreateProductCommandHander_ShouldReturn_CreateProductCommandResponse()
    {
        //Arrange
        var id = 1;
        var name = "product name";

        var productRepositoryMock = new Mock<IProductRepository>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
            .Returns(new Product
            {
                Id = id,
                Name = name,
            });

        var handler = new CreateProductCommandHandler(UnitOfWorkMock.Object, productRepositoryMock.Object, Mapper);

        //Act
        var response = await handler.HandleAsync(new CreateProductCommand(name));

        //Arrange
        response.Id.Should().Be(id);
        response.Name.Should().Be(name);
    }
}