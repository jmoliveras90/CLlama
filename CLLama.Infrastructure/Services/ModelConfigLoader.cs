namespace CLLama.Infrastructure.Services
{
    public static class ModelConfigLoader
    {
        public static Dictionary<string, string> LoadModelsFromCsv()
        {
            var models = new Dictionary<string, string>();

            // models.csv path
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string csvPath = Path.Combine(appPath, "models", "models.csv");

            // Check if the file exists
            if (!File.Exists(csvPath))
                throw new FileNotFoundException($"The file {csvPath} does not exist.");

            // Read CSV file line by line (omit header)
            var lines = File.ReadAllLines(csvPath);
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line)) 
                    continue;

                var parts = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                {
                    Console.WriteLine($"Invalid line in {csvPath}: {line}");
                    continue;
                }

                var key = parts[0].Trim();
                var url = parts[1].Trim();

                if (!models.ContainsKey(key))
                    models.Add(key, url);
            }

            return models;
        }
    }
}
