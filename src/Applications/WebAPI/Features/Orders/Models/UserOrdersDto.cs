namespace WebAPI.Features.Orders.Models
{
    public class UserOrdersDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public List<OrderDto> Orders { get; set; }
    }

}
