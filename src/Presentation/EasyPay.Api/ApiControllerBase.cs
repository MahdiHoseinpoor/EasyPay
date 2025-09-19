using EasyPay.Common;
using EasyPay.Common.Errors;
using Microsoft.AspNetCore.Mvc;

namespace EasyPay.Api
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected ActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return HandleFailure(result.error);
        }

        protected ActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return HandleFailure(result.error);
        }

        private ActionResult HandleFailure(Error error)
        {
            return error switch
            {
                ValidationError validationError =>
                    new BadRequestObjectResult(new ValidationProblemDetails(validationError.Errors)
                    {
                        Title = validationError.message,
                        Status = StatusCodes.Status400BadRequest
                    }),
                { code: 400 } => BadRequest(error),
                { code: 401 } => Unauthorized(error),
                { code: 403 } => Forbid(),
                { code: 404 } => NotFound(error),
                { code: 409 } => Conflict(error),
                DbUpdateError dbError => StatusCode(StatusCodes.Status400BadRequest, dbError),
                _ => StatusCode(StatusCodes.Status500InternalServerError, error)
            };
        }
    }
}