using AuthLab2.Infrastructure;
using AuthLab2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        public Cart? Cart { get; set; }

        public PredictModel()
        {
            _session = new InferenceSession("FRAUD.onnx");
        }

        [BindProperty]
        public Order Order { get; set; } = new Order();

        [BindProperty]
        public Customer Customer { get; set; } = new Customer();

        public string Prediction { get; set; }

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

        public IActionResult OnPost()
        {
            var features = new List<float>
            {
                Order.customer_ID,
                Order.time,
                Convert.ToSingle(Order.amount),
                int.Parse(Order.date),
                Order.entry_mode == "PIN" ? 1f : 0f,
                Order.country_of_transaction == "United Kingdom" ? 1f : 0f,
                Customer.country_of_residence == "United Kingdom" ? 1f : 0f,
            };

            var inputTensor = new DenseTensor<float>(features.ToArray(), new[] { 1, features.Count });
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", inputTensor)
            };

            using var results = _session.Run(inputs);
            var prediction = results.FirstOrDefault()?.AsTensor<float>().ToArray();

            Prediction = prediction?[0] >= 0.5 ? "Fraud" : "Not Fraud";

            if (Prediction == "Fraud")
            {
                return RedirectToPage("FraudOrder");
            }
            else
            {
                return RedirectToPage("NotFraudOrder");
            }
        }
    }
}
