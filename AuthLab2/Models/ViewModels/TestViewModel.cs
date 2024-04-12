using System.ComponentModel.DataAnnotations;

namespace AuthLab2.Models.ViewModels
{
    public class TestViewModel
    
    {
        public user_recommendations user_recommendations { get; set; }
        public Product Product { get; set; }
        public List<Product> uRecommendedProducts { get; set; }
        public bool IsUserLoggedIn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
