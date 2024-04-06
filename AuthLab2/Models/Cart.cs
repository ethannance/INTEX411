namespace AuthLab2.Models
{
    public class Cart
    {
        public List<CartLine> Lines { get; set; } = new List<CartLine>();

        public void AddItem(Book b, int quantity)
        {
            CartLine? line = Lines
                .Where(x => x.Book.BookId == b.BookId)
                .FirstOrDefault();

            //Has this item already been aded to our cart?
            if (line == null)
            {
                Lines.Add(new CartLine
                {
                    Book = b,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public void RemoveLine(Book b) => Lines.RemoveAll(x => x.Book.BookId == b.BookId);

        public void Clear() => Lines.Clear();

        public decimal CalculateTotal() => Lines.Sum(x => 25 * x.Quantity);

        public class CartLine
        {
            public int CartLineId { get; set; }
            public Book Book { get; set; }
            public int Quantity { get; set; }
        }
    }
}
