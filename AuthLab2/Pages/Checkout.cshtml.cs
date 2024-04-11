using AuthLab2.Infrastructure;
using AuthLab2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AuthLab2.Pages
{
    public class PredictModel : PageModel
    {
        private readonly InferenceSession _session;
        private readonly ILegoRepository _repo;
        private readonly IServiceProvider _serviceProvider; // Add IServiceProvider

        // Modify the constructor to accept IServiceProvider and ILegoRepository
        public PredictModel(ILegoRepository repo, IServiceProvider serviceProvider)
        {
            _session = new InferenceSession("FRAUD.onnx");
            _repo = repo;
            _serviceProvider = serviceProvider; // Initialize _serviceProvider
        }

        [BindProperty]
        public Order Order { get; set; } = new Order();

        [BindProperty]
        public Customer Customer { get; set; } = new Customer();

        public string Prediction { get; set; }

        public Cart? Cart { get; set; }

        public void OnGet()
        {
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            Order = new Order
            {
                amount = Cart.CalculateTotal(),
                time = DateTime.Now.Hour,
                date = DateTime.Now.Month.ToString()
            };
            Customer.country_of_residence = "United Kingdom";
        }

        public async Task<IActionResult> OnPost()
        {
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            Order.amount = Cart.CalculateTotal(); // Ensure the amount is updated based on the cart
            Order.date = DateTime.Now.ToString("yyyy-MM-dd"); // Assuming you want the current date
            Order.time = DateTime.Now.Hour;
            Order.day_of_week = DateTime.Now.DayOfWeek.ToString();
            Random random = new Random();
            Order.transaction_ID = random.Next(100000, 1000000);


            // Prepare features for fraud detection
            var features = new List<float>
            {
                Order.customer_ID,
                Order.time,
                Convert.ToSingle(Order.amount),
                DateTime.Now.DayOfYear, // Example if you need the day of year for date
                Order.entry_mode == "PIN" ? 1f : 0f,
                Order.country_of_transaction == "United Kingdom" ? 1f : 0f,
                Customer.country_of_residence == "United Kingdom" ? 1f : 0f,
            };

            var inputTensor = new DenseTensor<float>(features.ToArray(), new[] { 1, features.Count });
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", inputTensor)
            };

            // Run fraud detection
            using var results = _session.Run(inputs);
            var prediction = results.FirstOrDefault()?.AsTensor<float>().ToArray();

            Prediction = prediction?[0] >= 0.5 ? "Fraud" : "Not Fraud";
            Order.fraud = Prediction == "Fraud";

            // Save the order to the database
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LegoContext>();
                dbContext.Orders.Add(Order);
                await dbContext.SaveChangesAsync();
            }

            if (Order.fraud)
            {
                return RedirectToPage("FraudOrder");
            }
            else
            {
                HttpContext.Session.Remove("cart"); // Clear the cart session after successful order placement
                return RedirectToPage("NotFraudOrder", new { id = Order.transaction_ID });
            }
        }
    }
}
