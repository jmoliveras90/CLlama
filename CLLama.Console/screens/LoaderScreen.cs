using CLLama.Infrastructure.Services;
using LLama;

internal static class LoaderScreen
{
    public static async Task<InteractiveExecutor?> Show()
    {
        try {
            Console.Clear();
            Console.WriteLine("Available models:\n");

            // Read models from CSV file and show list of
            var models = ModelConfigLoader.LoadModelsFromCsv();
            foreach (var model in models)
                Console.Write($"{model.Key}  ");

            // Ask user to select a model
            Console.WriteLine("\n\nEnter the model you want load:");
            string? selectedModel = Console.ReadLine()?.Trim();

            if (selectedModel == null || !models.ContainsKey(selectedModel))
            {
                throw new ArgumentException("\nModel not found. Check name and try again.");
            }

            // Get URL of selected model
            string modelUrl = models[selectedModel];
            Console.WriteLine($"\nLoading model: {selectedModel}");

            // Download model selected
            string modelPath = await ModelDownloader.DownloadModelAsync(modelUrl);
            Console.WriteLine($"\nModel downloaded in: {modelPath}\n");

            // Return executor
            var loader = new ModelLoader();
            return loader.Load(modelPath);
        }
        catch (Exception ex) 
        { 
            ErrorHandler.Print(ex); 
        }

        return null;
    }
}
