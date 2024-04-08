namespace AuthLab2.Models.ViewModels
{
    // ViewModel for displaying a list of books along with pagination information
    public class ProductsListViewModel
    {
        //Books to display
        public IQueryable<Product> Products { get; set;}
        public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();

        public string? CurrentProductType { get; set; }
    }
}
