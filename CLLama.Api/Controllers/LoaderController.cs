using CLLama.Api.Dto;
using CLLama.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace CLLama.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaderController(ModelExecutorService executorService) : ControllerBase
    {
        [HttpGet("availablemodels")]
        public IActionResult GetAvailableModels()
        {
            var models = ModelConfigLoader.LoadModelsFromCsv();

            return Ok(models.Keys);
        }

        [HttpPost("load")]
        public async Task<IActionResult> LoadModel([FromBody] LoadModelDto request)
        {
            try
            {
                var models = ModelConfigLoader.LoadModelsFromCsv();

                if (request.SelectedModel == null || !models.ContainsKey(request.SelectedModel))
                {
                    return BadRequest(new { Message = "Model not found. Check name and try again." });
                }

                string modelUrl = models[request.SelectedModel];
                string modelPath = await ModelDownloader.DownloadModelAsync(modelUrl);
                var loader = new ModelLoader();
                var executor = loader.Load(modelPath);

                if (executor == null)
                    return StatusCode(500, new { Message = "Failed to load model." });

                executorService.SetExecutor(executor);

                return Ok(new { Message = $"Model '{request.SelectedModel}' loaded successfully.", ModelPath = modelPath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }
    }
}
