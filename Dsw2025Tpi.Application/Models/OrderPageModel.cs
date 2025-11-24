namespace Dsw2025Tpi.Application.Models
{
    public record PageModel<T>
    {
        public IEnumerable<T> elementsPage { set; get; }
        public string? pageNumber { set; get; }
        public string? pageSize { set; get; }
        public string? totalPages { set; get; }
    }
}
