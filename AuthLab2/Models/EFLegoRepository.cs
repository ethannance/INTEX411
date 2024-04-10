
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

        public void AddProduct(Product product) 
        { 
            _context.Products.Add(product);
            _context.SaveChanges();
        }
    }
}
