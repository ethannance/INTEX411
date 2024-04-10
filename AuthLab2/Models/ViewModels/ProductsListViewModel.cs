namespace AuthLab2.Models.ViewModels
{
    public class ProductsListViewModel
    {
        public IQueryable<Product> Products { get; set; }
        public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();
        public string? CurrentProductType { get; set; }
        public IEnumerable<string> ProductTypes { get; set; } = new List<string>();
        public string? CurrentProductColor { get; set; }
        public IEnumerable<string> ProductColors { get; set; } = new List<string>();

    }
}

