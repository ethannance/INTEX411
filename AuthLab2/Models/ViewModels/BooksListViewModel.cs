namespace AuthLab2.Models.ViewModels
{
    // ViewModel for displaying a list of books along with pagination information
    public class BooksListViewModel
    {
        //Books to display
        public IQueryable<Book> Books { get; set;}
        public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();

        public string? CurrentBookType { get; set; }
    }
}
