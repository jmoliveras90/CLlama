using LLama;
using LLama.Common;

namespace CLLama.Infrastructure.Services
{
    public class ModelLoader
    {
        public async Task Load(string modelPath)
        {
            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 1024, // The longest length of chat as memory.
                GpuLayerCount = 5 // How many layers to offload to GPU. Please adjust it according to your GPU memory.
            };
            using var model = LLamaWeights.LoadFromFile(parameters);
            using var context = model.CreateContext(parameters);
            var executor = new InteractiveExecutor(context);

            // Add chat histories as prompt to tell AI how to act.
            var chatHistory = new ChatHistory();
            chatHistory.AddMessage(AuthorRole.System, "You are an assistant named CLlama. Be helpful and concise. Always respond in the same language as the user's input.");

            // Example of history and model responses that respond based on accumulated history
            // WIP: Replacement with history saved in JSON files or SQLite DB 
            // chatHistory.AddMessage(AuthorRole.User, "Hello, CLlama.");
            // chatHistory.AddMessage(AuthorRole.Assistant, "Hello. How may I help you today?");

            ChatSession session = new(executor, chatHistory);

            InferenceParams inferenceParams = new InferenceParams()
            {
                MaxTokens = 256, // No more than 256 tokens should appear in answer. Remove it if antiprompt is enough for control.
                AntiPrompts = new List<string> { "User:" } // Stop generation once antiprompts appear.
            };

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Chat session started.\nUser: ");
            Console.ForegroundColor = ConsoleColor.Green;
            string userInput = Console.ReadLine() ?? "";

            while (userInput != "exit")
            {
                await foreach ( // Generate the response streamingly.
                    var text
                    in session.ChatAsync(
                        new ChatHistory.Message(AuthorRole.User, userInput),
                        inferenceParams))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(text);
                }
                Console.ForegroundColor = ConsoleColor.Green;
                userInput = Console.ReadLine() ?? "";
            }
        }
    }
}
