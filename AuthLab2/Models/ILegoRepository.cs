namespace AuthLab2.Models
{
    public interface ILegoRepository
    {
        public IQueryable<Product> Products { get; }
        public IQueryable<content_recs> content_Recs { get; }
        public IQueryable<user_recommendations> user_recommendations { get; }
    }
}
