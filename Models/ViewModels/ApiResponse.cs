namespace Project1.Models.ViewModels
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        // Add this parameterless constructor
        public ApiResponse() { }

        public ApiResponse(T data)
        {
            Success = true;
            Data = data;
        }

        public ApiResponse(string message)
        {
            Success = false;
            Message = message;
        }
    }

    public class ApiResponse : ApiResponse<object>
    {
        public ApiResponse(string message) : base(message) { }
    }

    public class ApiPagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        // Add this parameterless constructor
        public ApiPagedResponse() { }

        public ApiPagedResponse(
            IEnumerable<T> items,
            int totalCount,
            int page,
            int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }
    }
}
