using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PlanAppAPI.Core
{
    public class BaseApiController : ControllerBase
    {
        protected IMediator Mediator { get; }

        public BaseApiController(IMediator mediator)
        {
            Mediator = mediator;
        }

        protected ActionResult HandleResult<T>(Result<T>? result)
        {
            if (result == null)
            {
                return NotFound();
            }

            return result.IsSuccess switch
            {
                true when result.Value != null => Ok(new { data = result.Value }),
                true when result.Value == null => NotFound(),
                _ => BadRequest(result.Error)
            };
        }
    }
}