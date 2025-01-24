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

        [HttpPost]
        public async Task<IActionResult> GetOrders(IFormFile file, [FromQuery] int? orderId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if(orderId == null && (startDate == null || endDate == null))
                {
                    var problemDetails = ProblemDetailsExtensions
                                   .CreateProblemDetails(HttpContext, "Obrigatório o uso de Data Início e Data Fim quando não há um OrderId.", 
                                   $"StartDate: {startDate} - EndDate = {endDate}", StatusCodes.Status404NotFound);
                    return BadRequest(problemDetails);
                }
                
                var query = new GetOrdersQuery(file, orderId, startDate, endDate);
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
