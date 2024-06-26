﻿
using Microsoft.EntityFrameworkCore;

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
        public IQueryable<Order> Orders => _context.Orders;
        public IQueryable<Customer> Customers => _context.Customers;

        public IQueryable<content_recs> content_Recs => _context.content_recs;

        public IQueryable<user_recommendations> user_recommendations => _context.user_recommendations;
        public async Task<int> GetLastCustomerIdAsync()
        {
            // Query the Customers table to get the maximum customer ID
            var lastCustomerId = await _context.Customers.MaxAsync(c => (int?)c.customer_ID) ?? 0;

            return lastCustomerId;
        }

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
        public void DeleteOrder(Order order)
        {
            _context.Orders.Remove(order);
            _context.SaveChanges();
        }
        public void AddCustomerAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }
        public void EditUser(Customer customer)
        {
            _context.Customers.Update(customer);
            _context.SaveChanges();
        }
        public void DeleteUser(Customer customer)
        {
            _context.Customers.Remove(customer);
            _context.SaveChanges();
        }
        public void AddUser(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }

    }
}
