namespace AuthLab2.Models.ViewModels
{
    //Create variables for pagination specifications
    public class PaginationInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalNumPages => (int) (Math.Ceiling((decimal) TotalItems / ItemsPerPage));
    }
}
