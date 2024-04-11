using AuthLab2.Infrastructure;
using AuthLab2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthLab2.Pages
{
    public class PredictModel : PageModel
    {
        private readonly InferenceSession _session;
        private readonly ILegoRepository _repo;
        private readonly IServiceProvider _serviceProvider;

        public PredictModel(ILegoRepository repo, IServiceProvider serviceProvider)
        {
            _session = new InferenceSession("FRAUD.onnx");
            _repo = repo;
            _serviceProvider = serviceProvider;
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
                date = DateTime.Now.ToString("yyyy-MM-dd"),
                day_of_week = DateTime.Now.DayOfWeek.ToString()
            };
            Customer.country_of_residence = "United Kingdom";
        }

        public async Task<IActionResult> OnPost()
        {
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            Order.amount = Cart.CalculateTotal();
            Order.date = DateTime.Now.ToString("yyyy-MM-dd");
            Order.day_of_week = DateTime.Now.DayOfWeek.ToString();
            Order.time = DateTime.Now.Hour;

            // Generate a unique transaction_ID
            Random random = new Random();
            Order.transaction_ID = random.Next(100000, 1000000);

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LegoContext>();

                // Auto-increment customer_ID
                int lastCustomerId = await dbContext.Orders.MaxAsync(o => (int?)o.customer_ID) ?? 0;
                Order.customer_ID = lastCustomerId + 1;

                // Prepare features for fraud detection
                var features = new List<float>
                    {
                        Order.customer_ID,
                        Order.time,
                        Convert.ToSingle(Order.amount),
                        DateTime.Now.DayOfYear, // Assuming you need the day of the year for the fraud detection
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

                // Regardless of fraud prediction, save the order
                dbContext.Orders.Add(Order);
                await dbContext.SaveChangesAsync();
            }

            // Redirect based on fraud prediction
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
