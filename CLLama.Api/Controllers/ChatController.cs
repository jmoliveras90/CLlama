using CLLama.Api.Dto;
using CLLama.Infrastructure.Services;
using LLama;
using Microsoft.AspNetCore.Mvc;

namespace CLLama.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController(ModelExecutorService executorService) : ControllerBase
    {      
        [HttpPost("send")]
        public async Task<IActionResult> ChatWithModel([FromBody] ChatRequestDto request)
        {
            if (!executorService.IsModelLoaded)
            {
                return BadRequest(new { Message = "No model loaded. Please load a model first." });
            }

            var chatService = new ChatService(executorService.Executor!);
            chatService.InitializeChat();

            Response.Headers.Append("Content-Type", "text/event-stream");

            await foreach (var fragment in chatService.GetResponseStreamingAsync(request.UserInput))
            {
                await Response.WriteAsync(fragment);
                await Response.Body.FlushAsync();
            }

            return new EmptyResult();
        }
    }
}
