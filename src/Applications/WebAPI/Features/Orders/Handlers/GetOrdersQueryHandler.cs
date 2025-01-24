using MediatR;
using WebAPI.Features.Orders.Models;
using WebAPI.Features.Orders.Queries;
using WebAPI.Infrastructure.Interfaces.Helpers;

namespace WebAPI.Features.Orders.Handlers
{
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<UserOrdersDto>>
    {
        private readonly IFileReader _fileReader;

        public GetOrdersQueryHandler(IFileReader fileReader) 
        {
            _fileReader = fileReader;
        }
        public async Task<List<UserOrdersDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            List<string> allData = await _fileReader.ReadAllLinesAsync(request.file);

            var orders = new List<UserOrdersDto>();

            foreach (var line in allData)
            {
                try
                {
                    var userId = int.Parse(line.Substring(0, 10).TrimStart('0'));
                    var name = line.Substring(10, 45).Trim();
                    var orderId = int.Parse(line.Substring(55, 10).TrimStart('0'));
                    var productIdRaw = line.Substring(65, 10).TrimStart('0');
                    var productId = productIdRaw == string.Empty ? 0 : int.Parse(productIdRaw);
                    var value = ParseDecimalValue(line.Substring(75, 12));
                    var date = DateTime.ParseExact(line.Substring(87, 8), "yyyyMMdd", null);

                    var user = orders.FirstOrDefault(o => o.UserId == userId);
                    if (user == null)
                    {
                        user = new UserOrdersDto
                        {
                            UserId = userId,
                            Name = name,
                            Orders = new List<OrderDto>()
                        };
                        orders.Add(user);
                    }

                    var order = user.Orders.FirstOrDefault(o => o.OrderId == orderId);
                    if (order == null)
                    {
                        order = new OrderDto
                        {
                            OrderId = orderId,
                            Total = 0,
                            Date = date,
                            Products = new List<ProductDto>()
                        };
                        user.Orders.Add(order);
                    }

                    order.Total += value;
                    order.Products.Add(new ProductDto { ProductId = productId, Value = value });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar linha: '{line}' - {ex.Message}");
                }
            }
            
            if (request.OrderId.HasValue)
            {
                orders.ForEach(user => user.Orders = user.Orders.Where(o => o.OrderId == request.OrderId).ToList());
            }

            //Só procuro por data quando existem os 2 dados
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                orders.ForEach(user => user.Orders = user.Orders
                    .Where(o => o.Date >= request.StartDate && o.Date <= request.EndDate).ToList());
            }

            return orders.Where(u => u.Orders.Any()).ToList();
        }

        public decimal ParseDecimalValue(string rawValue)
        {
            if (decimal.TryParse(rawValue.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var value))
                return Math.Round(value, 2);             
            throw new FormatException($"Valor decimal inválido: {rawValue}");
        }
    }
}
