using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;
using WebAPI.Features.Orders.Models;
using WebAPI.Features.Orders.Queries;
using Xunit;

namespace Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<OrdersController>> _loggerMock;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<OrdersController>>();
            _controller = new OrdersController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetOrders_ShouldReturnOk_WhenOrdersExist()
        {
            // Arrange
            var orders = new List<UserOrdersDto>
            {
                new UserOrdersDto
                {
                    UserId = 1,
                    Name = "User 1",
                    Orders = new List<OrderDto>
                    {
                        new OrderDto
                        {
                            OrderId = 1,
                            Total = 100.0M,
                            Date = DateTime.Now,
                            Products = new List<ProductDto>
                            {
                                new ProductDto { ProductId = 1, Value = 50.0M },
                                new ProductDto { ProductId = 2, Value = 50.0M }
                            }
                        }
                    }
                },
                new UserOrdersDto
                {
                    UserId = 2,
                    Name = "User 2",
                    Orders = new List<OrderDto>
                    {
                        new OrderDto
                        {
                            OrderId = 2,
                            Total = 200.0M,
                            Date = DateTime.Now,
                            Products = new List<ProductDto>
                            {
                                new ProductDto { ProductId = 3, Value = 200.0M }
                            }
                        }
                    }
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetOrdersQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(orders);

            // Act
            var result = await _controller.GetOrders(null, null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrders = Assert.IsAssignableFrom<IEnumerable<UserOrdersDto>>(okResult.Value);
            Assert.Equal(orders.Count, returnedOrders.Count());
        }

        [Fact]
        public async Task GetOrders_ShouldReturnNotFound_WhenNoOrdersExist()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetOrdersQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new List<UserOrdersDto>());

            // Act
            var result = await _controller.GetOrders(null, null, null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetOrders_ShouldReturnBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetOrdersQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetOrders(null, null, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsAssignableFrom<ProblemDetails>(badRequestResult.Value);
            Assert.Equal("Falha ao retornar os dados do arquivo", problemDetails.Title);
        }
    }
}