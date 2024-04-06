namespace AuthLab2.Models
{
    public interface IBookRepository
    {
        public IQueryable<Book> Books { get; }
    }
}
