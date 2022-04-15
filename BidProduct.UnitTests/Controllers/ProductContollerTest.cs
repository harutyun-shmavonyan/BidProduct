using System.Threading.Tasks;
using BidProduct.API.Controllers;
using BidProduct.API.ViewModels.Product;
using BidProduct.Common.Abstract;
using BidProduct.SL.Models.CQRS.Commands;
using BidProduct.SL.Models.CQRS.Queries;
using BidProduct.SL.Models.CQRS.Responses;
using BidProduct.UnitTests.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BidProduct.UnitTests.Controllers
{
    [TestFixture]
    public class ProductContollerTest : ControllerTest
    {
        [Test]
        public async Task GetExistingProduct_ShouldReturnProduct()
        {
            //Arrange
            var id = 1;
            var name = "12345";

            AddMockResponse<GetProductQuery, GetProductQueryResponse>(new GetProductQueryResponse()
            {
                Id = id,
                Name = name
            });

            var controller = new ProductController(Mediator, Mapper);

            //Act
            var response = await controller.GetAsync(1);

            //Assert
            response.Should().NotBeNull();
            response.Result.Should().BeOfType<OkObjectResult>();

            var productReadViewModel = (response.Result as OkObjectResult)?.Value as ProductReadViewModel;

            productReadViewModel.Should().NotBeNull();
            productReadViewModel!.Id.Should().Be(id);
            productReadViewModel.Name.Should().Be(name);
        }

        [Test]
        public async Task CreateProduct_ShouldReturnCreated()
        {
            //Arrange
            ServiceCollection.AddSingleton<IDateTimeService, FixedDateTimeService>();

            AddMockResponse<CreateProductCommand, CreateProductCommandResponse>(new CreateProductCommandResponse());

            var controller = new ProductController(Mediator, Mapper);

            //Act
            var response = await controller.CreateAsync(new ProductCreateViewModel());
            
            //Assert
            response.Should().NotBeNull();
            response.Result.Should().BeOfType<CreatedAtActionResult>();
        }
    }
}
