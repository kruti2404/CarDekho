using System.ComponentModel;
namespace Task1.DTO
{
    public enum StockAvailability
    {
        [Description("In Stock")]
        InStock,

        [Description("Out of stock")]
        OutOfStock,

        [Description("All")]
        All
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                             .FirstOrDefault() as DescriptionAttribute;
            return attr?.Description ?? value.ToString();
        }
    }
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
        public StockAvailability StockAvail { get; set; } = StockAvailability.All;
    }
}
