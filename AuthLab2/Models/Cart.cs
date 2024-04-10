
namespace AuthLab2.Models
{
    public class Cart
    {
        public List<CartLine> Lines { get; set; } = new List<CartLine>();

        public void AddItem(Product p, int quantity)
        {
            CartLine? line = Lines
                .Where(x => x.Product.product_ID == p.product_ID)
                .FirstOrDefault();

            //Has this item already been aded to our cart?
            if (line == null)
            {
                Lines.Add(new CartLine
                {
                    Product = p,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public void RemoveLine(Product b) => Lines.RemoveAll(x => x.Product.product_ID == b.product_ID);

        public void Clear() => Lines.Clear();

        public decimal CalculateTotal() => Lines.Sum(x => x.Product.price * x.Quantity);

        public void DecreaseItemQuantity(Product product, int quantity)
        {
            var line = Lines.FirstOrDefault(l => l.Product.product_ID == product.product_ID);

            if (line != null)
            {
                line.Quantity -= quantity;
                if (line.Quantity <= 0)
                {
                    Lines.RemoveAll(l => l.Product.product_ID == product.product_ID);
                }
            }
        }


        public class CartLine
        {
            public int CartLineId { get; set; }
            public Product Product { get; set; }
            public int Quantity { get; set; }
        }
    }
}
