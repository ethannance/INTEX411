namespace AuthLab2.Models
{
    public class ProductEqualityComparer : IEqualityComparer<Product>
    {
        public bool Equals(Product x, Product y) //Makes sure to remove duplicates from product list
        {
            
            return x.name == y.name;
        }

        public int GetHashCode(Product obj) //Required by IEqualityComparer
        {
            
            return obj.name.GetHashCode();
        }
    }
}
