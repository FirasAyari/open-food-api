namespace open_food_api.Infrastructure.Services.DTOs
{
    public class PaginationResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }

        public PaginationResult(List<T> items, int totalCount)
        {
            Items = items;
            TotalCount = totalCount;
        }
    }
}
