﻿namespace AuthLab2.Models
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

        public decimal CalculateTotal() => Lines.Sum(x => 25 * x.Quantity);

        public class CartLine
        {
            public int CartLineId { get; set; }
            public Product Product { get; set; }
            public int Quantity { get; set; }
        }
    }
}
