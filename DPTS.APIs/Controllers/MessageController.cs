using DPTS.APIs.Models;
using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using Microsoft.AspNetCore.Mvc;

namespace DPTS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentMessages([FromQuery] RecentMessageIndexModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _messageService.GetRecentMessagesAsync(
                model.SellerId,
                model.PageNumber,
                model.PageSize);

            return HandleResult(result);
        }

        private IActionResult HandleResult<T>(ServiceResult<T> result)
        {
            return result.Status switch
            {
                StatusResult.Success => Ok(new { result.MessageResult, result.Data }),
                StatusResult.Warning => Ok(new { result.MessageResult, result.Data }),
                StatusResult.Failed => NotFound(result.MessageResult),
                StatusResult.Errored => StatusCode(500, result.MessageResult),
                _ => BadRequest("Trạng thái không xác định.")
            };
        }
    }
}
