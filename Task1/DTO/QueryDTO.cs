namespace Task1.DTO
{
    public class QueryDTO
    {
        public string SearchTerm { get; set; } = "";
        public int PageSize { get; set; } = 20;
        public int PageNumber { get; set; } = 1;
        public string SortColumn { get; set; } = "Id";
        public string SortDirection { get; set; } = "ASC";
        public string SingleFilter { get; set; } = "";
        public string MultiFilter { get; set; } = "";
        public int MinPrice { get; set; } = 200000;
        public int MaxPrice { get; set; } = 200000000;

    }
}
