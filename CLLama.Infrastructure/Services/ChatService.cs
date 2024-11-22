using LLama;
using LLama.Common;

namespace CLLama.Infrastructure.Services
{
    public class ChatService
    {
        private readonly InteractiveExecutor _executor;
        private readonly ChatHistory _chatHistory;

        public ChatService(InteractiveExecutor executor)
        {
            _executor = executor;
            _chatHistory = new ChatHistory();
        }

        public void InitializeChat()
        {
            string system_prompt = "Transcript of a dialog, where the User interacts with an Assistant. " 
                                 + "Assistant is helpful, kind, honest, good at writing, and never fails " 
                                 + "to answer the User's requests immediately and with precision. " 
                                 + "Always respond in the same language as the user's input.";

            _chatHistory.AddMessage(AuthorRole.System, system_prompt);

            // WIP: add history from JSON file or DB
            //_chatHistory.AddMessage(AuthorRole.User, "Hello");
            //_chatHistory.AddMessage(AuthorRole.Assistant, "Hello. How may I help you today?");
        }

        public async IAsyncEnumerable<string> GetResponseStreamingAsync(string userInput, int maxTokens = 512)
        {
            var inferenceParams = new InferenceParams
            {
                MaxTokens = maxTokens,
                AntiPrompts = new List<string> { "\nUser: " }
            };

            ChatSession session = new(_executor, _chatHistory);

            // Response stream from the model
            string response = "";
            await foreach (var text in session.ChatAsync(new ChatHistory.Message(AuthorRole.User, userInput), inferenceParams))
            {
                response += text;
                yield return text; // Return response fragments
            }

            // Add the full response to history
            _chatHistory.AddMessage(AuthorRole.Assistant, response);
        }
    }
}
