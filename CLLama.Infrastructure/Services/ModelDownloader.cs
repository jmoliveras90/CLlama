using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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

                // Verificar si el modelo ya existe
                if (File.Exists(filePath))
                {
                    Console.WriteLine($"El modelo ya existe en: {filePath}");
                    return filePath;
                }

                // Descargar el modelo
                using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                // Obtener tamaño del archivo (si está disponible)
                long totalBytes = response.Content.Headers.ContentLength ?? -1;
                long bytesDownloaded = 0;

                using var contentStream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

                byte[] buffer = new byte[8192];
                int bytesRead;

                Console.WriteLine("Descargando modelo...");
                Console.WriteLine("░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░");
                Console.WriteLine(" Progreso: 0.00%");

                // Posiciones iniciales de la barra y progreso
                int progressBarLine = Console.CursorTop - 2;
                int progressTextLine = Console.CursorTop - 1;

                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    bytesDownloaded += bytesRead;

                    // Calcular progreso
                    if (totalBytes > 0)
                    {
                        double progress = (double)bytesDownloaded / totalBytes * 100;

                        // Actualizar barra de progreso
                        int barWidth = 50; // Número de caracteres en la barra
                        int filledWidth = (int)(barWidth * bytesDownloaded / totalBytes);
                        string progressBar = new string('█', filledWidth) + new string('░', barWidth - filledWidth);

                        // Validar posiciones antes de mover el cursor
                        if (progressBarLine >= 0)
                        {
                            Console.SetCursorPosition(0, progressBarLine);
                            Console.Write(progressBar);
                        }

                        if (progressTextLine >= 0)
                        {
                            Console.SetCursorPosition(0, progressTextLine);
                            Console.Write($" Progreso: {progress:F2}%    ");
                        }
                    }
                    else
                    {
                        Console.Write($"\rProgreso: {bytesDownloaded} bytes descargados");
                    }
                }

                Console.WriteLine("\nDescarga completada.");
                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al descargar el modelo desde {url}: {ex.Message}");
                throw;
            }
        }
    }
}
