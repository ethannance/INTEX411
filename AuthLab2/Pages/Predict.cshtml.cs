using AuthLab2.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace AuthLab2.Pages
{
    public class PredictModel : PageModel
    {
        // Assuming you have a repository or some data service to fetch books
        private readonly ILegoRepository _bookRepository;

        public PredictModel(ILegoRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // Define a property to hold your book predictions
        public List<ProductPrediction> BookPredictions { get; set; }

        public void OnGet()
        {
            // Initialize your BookPredictions list here
            // This example just initializes an empty list for demonstration purposes
            // In practice, you would fetch and populate actual predictions based on your logic
            BookPredictions = new List<ProductPrediction>();

            // Example: Populate BookPredictions with data from your repository
            // This is a placeholder loop to demonstrate how you might convert book data to predictions
            foreach (var book in _bookRepository.Products)
            {
                BookPredictions.Add(new ProductPrediction
                {
                    Product = book,
                    Prediction = "YourPredictionLogicHere" // Replace with actual prediction logic
                });
            }
        }
    }
}