using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthLab2.Models
{
    public partial class user_recommendation
    {
        [Key]
        public int customer_ID { get; set; }

        public int if_you_liked { get; set; }

        public int Recommendation_1 { get; set; }

        public int Recommendation_2 { get; set; }

        public int Recommendation_3 { get; set; }

        public int Recommendation_4 { get; set; }

        public int Recommendation_5 { get; set; }
    }
}
