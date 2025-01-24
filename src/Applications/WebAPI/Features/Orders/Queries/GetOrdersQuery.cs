using MediatR;
using WebAPI.Features.Orders.Models;

namespace WebAPI.Features.Orders.Queries
{
    public record GetOrdersQuery(IFormFile file, int? OrderId, DateTime? StartDate, DateTime? EndDate) : IRequest<List<UserOrdersDto>>;
}
