using BidProduct.API.Controllers;
using BidProduct.SL.Models.CQRS.Queries;
using BidProduct.SL.Models.CQRS.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using BidProduct.API.ViewModels.Product;
using BidProduct.SL.Models.CQRS.Commands;
using BidProduct.Common.Abstract;
using BidProduct.UnitTests.Services;
using Moq;

namespace BidProduct.UnitTests.ControllerTests
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

            AddMockResponse<CreateProductCommand, CreateProductCommandResponse>(It.IsAny<CreateProductCommandResponse>());

            var controller = new ProductController(Mediator, Mapper);

            //Act
            var response = await controller.CreateAsync(It.IsAny<ProductCreateViewModel>());
            
            //Assert
            response.Should().NotBeNull();
            response.Result.Should().BeOfType<CreatedAtActionResult>();
        }
    }
}
