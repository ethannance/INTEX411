namespace AuthLab2.Models
{
    public interface ILegoRepository
    {
        public IQueryable<Product> Books { get; }
    }
}
