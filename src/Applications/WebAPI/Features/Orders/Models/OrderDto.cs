namespace WebAPI.Features.Orders.Models
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
        public List<ProductDto> Products { get; set; }
    }
}
