using DPTS.Applications.Shareds;
using Microsoft.AspNetCore.Mvc;

namespace DPTS.APIs
{
    public static class APIResult
    {
        public static IActionResult From<T>(ServiceResult<T> result)
        {
            return result.Status switch
            {
                StatusResult.Success => new OkObjectResult(result),
                StatusResult.Warning => new OkObjectResult(new
                {
                    result.MessageResult,
                    result.Status
                }),
                StatusResult.Failed => new BadRequestObjectResult(result),
                StatusResult.Errored => new ObjectResult(result)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                },
                _ => new NotFoundResult()
            };
        }
    }

}