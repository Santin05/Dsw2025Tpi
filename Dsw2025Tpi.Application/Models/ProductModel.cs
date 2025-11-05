namespace Dsw2025Tpi.Application.Dtos
{
    public record ProductModel
    {
        public record Request(string? Sku, string? Name, string? Description, string? InternalCode, decimal? CurrentUnitPrice, int? StockQuantity);
        public record Response(Guid Id);
    }
}
