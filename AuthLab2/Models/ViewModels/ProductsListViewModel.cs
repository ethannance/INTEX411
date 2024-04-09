namespace AuthLab2.Models.ViewModels
{
    // ViewModel for displaying a list of books along with pagination information
    public class ProductsListViewModel
    {
        public IQueryable<Product> Products { get; set; }
        public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();
        public string? CurrentProductType { get; set; }

        // Add this line
        public IEnumerable<string> ProductTypes { get; set; } = new List<string>();
    }

}
