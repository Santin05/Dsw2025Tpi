namespace Dsw2025Tpi.Application.Models
{
    public record OrderModel
    {
        public record Item(Guid productId, int quantity, string name, string description, decimal currentUnitPrice);
        public record Request(Guid customerId, string shippingAddress, string billingAddress, string? notes, IEnumerable<Item> orderItems);
        public record PageRequest(string? status);
        public record Response(Guid Id);
    }
}