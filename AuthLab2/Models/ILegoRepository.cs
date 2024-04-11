namespace AuthLab2.Models
{
    public interface ILegoRepository
    {
        public IQueryable<Product> Products { get; }
        public IQueryable<Order> Orders { get; }
        public IQueryable<Customer> Customers { get; }
        public IQueryable<content_recs> content_Recs { get; }
        public IQueryable<user_recommendations> user_recommendations { get; }
        public void AddProduct(Product product);
        public void EditProduct(Product product);
        public void DeleteProduct(Product product);
        public void DeleteOrder(Order order);
    }
}
