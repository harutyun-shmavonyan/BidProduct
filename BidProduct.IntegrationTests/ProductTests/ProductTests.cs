using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BidProduct.Common.Abstract;
using BidProduct.DAL.Abstract.Filtering;
using BidProduct.DAL.Models;
using BidProduct.DAL.Models.Filters;
using BidProduct.SL.Models.CQRS.Commands;
using BidProduct.SL.Models.CQRS.Queries;
using BidProduct.SL.Models.CQRS.Responses;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BidProduct.IntegrationTests.ProductTests
{
    [TestFixture]
    public class ProductTests : IntegrationTestBase
    {
        [Test]
        public async Task ProductAdd_ShouldAddToDb()
        {
            //Arrange
            var fixedDateTimeService = ServiceProvider.GetRequiredService<IDateTimeService>();

            var createProductCommand = new CreateProductCommand("TestName");

            //Act
            var id = (await Mediator.SendAsync(createProductCommand)).Id;
            var getProductQuery = new GetProductQuery(id: id);

            var productResponse = await Mediator.SendAsync(getProductQuery);

            //Assert
            productResponse.Should().NotBeNull();
            productResponse.Created.Should().Be(fixedDateTimeService.UtcNow);
            productResponse.Name.Should().Be(createProductCommand.Name);
            productResponse.Id.Should().Be(id);
        }

        [Test]
        public async Task ProductFilteringByName_ShouldWork()
        {
            //Arrange
            var commands = new CreateProductCommand[]
            {
                new("Table"),
                new("Keyboard"),
                new("Key"),
            };

            foreach (var createProductCommand in commands)
            {
                await Mediator.SendAsync(createProductCommand);
            }

            //Act
            var getProductsQuery = new GetByFilterQuery<Product, long, GetProductQueryResponse>(new ProductFilter
            {
                NamePrefix = "Key"
            });

            var productsResponses = await Mediator.SendAsync(getProductsQuery);

            //Assert
            productsResponses.Should().HaveCount(2);
            productsResponses.Should().Contain(p => p.Name == "Key");
            productsResponses.Should().Contain(p => p.Name == "Keyboard");
        }

        [Test]
        public async Task ProductOrderingByName_ShouldWork()
        {
            //Arrange
            var commands = new CreateProductCommand[]
            {
                new("3412"),
                new("2341"),
                new("4123"),
                new("1234"),
            };

            foreach (var createProductCommand in commands)
            {
                await Mediator.SendAsync(createProductCommand);
            }

            //Act
            var getProductsQuery = new GetByFilterQuery<Product, long, GetProductQueryResponse>(new ProductFilter
            {
                OrderedProperties = { new OrderedProperty<Product>(OrderingForm.Ascending, p => p.Name) }
            });

            var productsResponses = await Mediator.SendAsync(getProductsQuery);

            //Assert
            productsResponses.Should().HaveCount(4);
            productsResponses.Should().BeInAscendingOrder(p => p.Name);
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            await TruncateAsync<Product>();
        }
    }
}
