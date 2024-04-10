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

        public PredictModel()
        {
            // Initialize the inference session with the path to your ONNX model
            _session = new InferenceSession("FRAUD.onnx");
        }

        [BindProperty]
        public Order Order { get; set; }

        public Customer Customer { get; set; }

        public string Prediction { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Directly using the month from the form, assuming it's been adjusted to be an integer.
            int month_of_the_year = int.Parse(Order.date); // Make sure to validate this to avoid exceptions.

            string country_of_residence = Customer?.country_of_residence ?? "United Kingdom";

            var features = new List<float>
            {
                Order.customer_ID,
                Order.time,
                Order.amount,
                month_of_the_year,
                Order.entry_mode == "PIN" ? 1f : 0f,
                Order.country_of_transaction == "United Kingdom" ? 1f : 0f,
                country_of_residence == "United Kingdom" ? 1f : 0f,
            };

            // Convert the feature list to a tensor, assuming the model expects a single instance with a feature vector
            var inputTensor = new DenseTensor<float>(features.ToArray(), new[] { 1, features.Count });
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", inputTensor)
            };

            // Run the model with the prepared input
            using var results = _session.Run(inputs);

            // Extract the prediction result, assuming a single output from the model
            var prediction = results.FirstOrDefault()?.AsTensor<float>().ToArray();

            // Interpret the model's prediction (assuming binary classification: 0 for "Not Fraud", 1 for "Fraud")
            Prediction = prediction?[0] >= 0.5 ? "Fraud" : "Not Fraud";

            return Page();
        }
    }
}

