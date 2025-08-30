namespace Discount.gRPC.Models
{
    public class Cupon
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = default!;
        public string Desription { get; set; } = default!;
        public int Amount { get; set; }
    }
}
