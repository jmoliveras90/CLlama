using CLLama.Infrastructure.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

class Program
{
    static async Task Main(string[] args)
    {
        try {
            // Read models from CSV file
            var models = ModelConfigLoader.LoadModelsFromCsv();

            // Show list of available models
            Console.WriteLine("Available models:\n");

            foreach (var model in models)
                Console.Write($"{model.Key}  ");

            // Ask the user to select a model
            Console.WriteLine("\n\nEnter the model you want load:");
            string? selectedModel = Console.ReadLine()?.Trim();

            if (selectedModel == null || !models.ContainsKey(selectedModel))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nModel not found. Check name and try again.");
                Console.ResetColor();
                return;
            }

            // Get URL of selected model
            string modelUrl = models[selectedModel];

            // Step 1: Download the model
            Console.WriteLine($"\nLoading model: {selectedModel}");
            string modelPath = await ModelDownloader.DownloadModelAsync(modelUrl);

            Console.WriteLine($"\nModel downloaded in: {modelPath}\n");

            // Step 2: Load downloaded model
            var loader = new ModelLoader();
            await loader.Load(modelPath);

            Console.WriteLine("\nInteraction ended.\n");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError: {ex.Message}");
            Console.ResetColor();
        }
    }
}
