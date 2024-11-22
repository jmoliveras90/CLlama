namespace CLLama.Infrastructure.Services
{
    public static class ModelDownloader
    {
        public static async Task<string> DownloadModelAsync(string url)
        {
            using var client = new HttpClient();
            try {
                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                string modelsPath = Path.Combine(appPath, "models");

                if (!Directory.Exists(modelsPath))
                    Directory.CreateDirectory(modelsPath);

                string fileName = Path.GetFileName(new Uri(url).LocalPath);
                string filePath = Path.Combine(modelsPath, fileName);

                // Check if the model already exists
                if (File.Exists(filePath))
                {
                    Console.WriteLine($"The model already exists in: {filePath} \n");
                    return filePath;
                }

                // Download the model
                using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                // Get file size (if available)
                long totalBytes = response.Content.Headers.ContentLength ?? -1;
                long bytesDownloaded = 0;

                using var contentStream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

                byte[] buffer = new byte[8192];
                int bytesRead;

                Console.WriteLine("\nDownloading...");
                Console.WriteLine("░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░");
                Console.WriteLine("Progress: 0.00% ");

                // Initial bar positions and progress
                int progressBarLine = Console.CursorTop - 2;
                int progressTextLine = Console.CursorTop - 1;

                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    bytesDownloaded += bytesRead;

                    if (totalBytes > 0)
                    {
                        double progress = (double)bytesDownloaded / totalBytes * 100;

                        // Update progress bar
                        int barWidth = 50; // Number of characters in the bar (50)
                        int filledWidth = (int)(barWidth * bytesDownloaded / totalBytes);
                        string progressBar = new string('█', filledWidth) + new string('░', barWidth - filledWidth);

                        // Validate positions before moving the cursor
                        if (progressBarLine >= 0)
                        {
                            Console.SetCursorPosition(0, progressBarLine);
                            Console.Write(progressBar);
                        }

                        if (progressTextLine >= 0)
                        {
                            Console.SetCursorPosition(0, progressTextLine);
                            Console.Write($"Progress: {progress:F2}% ");
                        }
                    }
                    else
                    {
                        Console.Write($"\rProgress: {bytesDownloaded} bytes downloaded");
                    }
                }

                Console.WriteLine("\n -> Download completed.");
                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading model from {url}: {ex.Message}");
                throw;
            }
        }
    }
}
