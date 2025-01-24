using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using WebAPI.Features.Orders.Models;
using WebAPI.Features.Orders.Queries;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] int? orderId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var query = new GetOrdersQuery(orderId, startDate, endDate);
                var result = await _mediator.Send(query);
                if (result.Any())
                    return Ok(result);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                var problemDetails = ProblemDetailsExtensions
                                   .CreateProblemDetails(HttpContext, "Falha ao retornar os dados do arquivo", ex.Message, StatusCodes.Status404NotFound);
                return BadRequest(problemDetails);
            }
        }
    }
}
