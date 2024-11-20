// using EmailCollector.Api.DTOs;
// using EmailCollector.Api.Services.Users;
// using Microsoft.AspNetCore.Mvc;
//
// namespace EmailCollector.Api.Controllers;
//
// [ApiController]
// [Route("api/[controller]")]
// [ApiExplorerSettings(IgnoreApi = true)] // Exclude this controller from Swagger
// public class ApiKeysController : ControllerBase
// {
//     private readonly IApiKeyService _apiKeyService;
//
//     public ApiKeysController(IApiKeyService apiKeyService)
//     {
//         _apiKeyService = apiKeyService;
//     }
//
//     [HttpPost("generate")]
//     public async Task<ActionResult<ApiKeyCreatedDto>> GenerateApiKey([FromBody] GenerateApiKeyRequest request)
//     {
//         var result = await _apiKeyService.GenerateApiKeyAsync(request.UserId, request.Name, request.Expiration);
//         return Ok(result);
//     }
//
//     [HttpPost("revoke/{id}")]
//     public async Task<IActionResult> RevokeApiKey(Guid id)
//     {
//         await _apiKeyService.RevokeApiKeyAsync(id);
//         return NoContent();
//     }
// }
//
// public class GenerateApiKeyRequest
// {
//     public string UserId { get; set; }
//     public string Name { get; set; }
//     public DateTime? Expiration { get; set; }
// }
