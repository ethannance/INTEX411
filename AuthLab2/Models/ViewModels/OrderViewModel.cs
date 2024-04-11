namespace AuthLab2.Models.ViewModels
{
    public class OrderViewModel
    {
        public int transaction_ID { get; set; }
        public string date { get; set; }
        public int amount { get; set; }
        public string shipping_address { get; set; }
        public bool fraud { get; set; }
    }

}
