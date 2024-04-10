using System.ComponentModel.DataAnnotations;

namespace AuthLab2.Models
{
    public partial class content_recs
    {
        [Key]
        public int ProductID { get; set; }
        public string If_you_liked { get; set; }
        public string Recommendation_1 { get; set; }
        public string Recommendation_2 { get; set; }
        public string Recommendation_3 { get; set; }
        public string Recommendation_4 { get; set; }
        public string Recommendation_5 { get; set; }
    }
}
