
namespace AuthLab2.Models
{
    public class EFLegoRepository : ILegoRepository
    {
        private LegoContext _context;
        public EFLegoRepository(LegoContext temp)
        {
            _context = temp;
        }
        public IQueryable<Product> Products => _context.Products;

        public IQueryable<content_recs> content_Recs => _context.content_recs;

        public IQueryable<user_recommendations> user_recommendations => _context.user_recommendations;

        public void AddProduct(Product product) 
        { 
            _context.Products.Add(product);
            _context.SaveChanges();
        }
        public void EditProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }
        public void DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
    }
}
