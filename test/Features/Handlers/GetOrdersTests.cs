using WebAPI.Features.Orders.Handlers;
using WebAPI.Features.Orders.Queries;
using WebAPI.Features.Orders.Models;
using Moq;
using Xunit;
using WebAPI.Infrastructure.Interfaces.Helpers;

namespace WebAPI.Tests.Features.GetOrders
{
    public class GetOrdersTests
    {
        private readonly Mock<IFileReader> _fileReader;
        public GetOrdersTests() 
        {
            _fileReader = new Mock<IFileReader>();
        }

        [Fact]
        public void ParseDecimalValue_ValidInput_ReturnsCorrectValue()
        {
            var mockFileReader = new Mock<IFileReader>();
            var handler = new GetOrdersQueryHandler(_fileReader.Object);
            var rawValue = "1836.74";

            var result = handler.ParseDecimalValue(rawValue);

            Assert.Equal(1836.74m, result);
        }

        [Fact]
        public void ParseDecimalValue_InvalidInput_ThrowsFormatException()
        {
            var mockFileReader = new Mock<IFileReader>();
            var handler = new GetOrdersQueryHandler(_fileReader.Object);
            var rawValue = "invalid";

            Assert.Throws<FormatException>(() => handler.ParseDecimalValue(rawValue));
        }

        [Fact]
        public async Task GetOrders_FilterByOrderId_ReturnsFilteredOrders()
        {
            List<string> orders = GetOrders();

            _fileReader.Setup(x => x.ReadAllLinesAsync()).ReturnsAsync(orders);
            var handler = new GetOrdersQueryHandler(_fileReader.Object);
            var query = new GetOrdersQuery(123, null, null);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Single(result);
            Assert.Single(result.First().Orders);
            Assert.Equal(123, result.First().Orders.First().OrderId);
        }

        [Fact]
        public async Task GetOrders_FilterByDateRange_ReturnsFilteredOrders()
        {
            List<string> orders = GetOrders();
            _fileReader.Setup(x => x.ReadAllLinesAsync()).ReturnsAsync(orders);

            var handler = new GetOrdersQueryHandler(_fileReader.Object);
            var query = new GetOrdersQuery(null, DateTime.Parse("2021-11-25"), DateTime.Parse("2021-12-31"));

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Single(result);
            Assert.NotEmpty(result.First().Orders);
        }

        [Fact]
        public async Task GetOrders_NoData_ReturnsEmptyList()
        {
            _fileReader.Setup(x => x.ReadAllLinesAsync()).ReturnsAsync(new List<string>());

            var handler = new GetOrdersQueryHandler(_fileReader.Object);
            var query = new GetOrdersQuery(null, null, null);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Empty(result);
        }
        private static List<string> GetOrders()
        {
            List<string> orders = new List<string>();
            orders.Add("0000000070                              Palmer Prosacco00000001230000000003     1836.7420210308");
            orders.Add("0000000075                                  Bobbie Batz00000007980000000002     1578.5720211116");
            orders.Add("0000000014                                 Clelia Hills00000001460000000001      673.4920211125");
            return orders;
        }
    }
}
