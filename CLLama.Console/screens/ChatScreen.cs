using CLLama.Infrastructure.Services;
using LLama;

internal static class ChatScreen
{
    public static async Task Show(InteractiveExecutor executor)
    {
        try {
            var chatService = new ChatService(executor);
            chatService.InitializeChat();

            Console.Clear();
            Console.WriteLine("\nChat started. Type 'exit' to finish.");
            Console.Write("\nUser: ");

            string? userInput;
            do {
                Console.ForegroundColor = ConsoleColor.Green;
                userInput = Console.ReadLine();
                if (userInput == null)
                    continue;

                if (userInput.ToLower() != "exit")
                {
                    Console.ForegroundColor = ConsoleColor.White;

                    // Process the streaming response
                    await foreach (var text in chatService.GetResponseStreamingAsync(userInput))
                    {
                        Console.Write(text); // Print fragments as they arrive
                    }
                }
            }
            while (userInput?.ToLower() != "exit");

            Console.ResetColor();
            Console.WriteLine("End session.");
        }
        catch (Exception ex)
        {
            ErrorHandler.Print(ex);
        }
    }
}
