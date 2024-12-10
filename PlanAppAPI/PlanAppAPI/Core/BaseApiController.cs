using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PlanAppAPI.Core;

public class BaseApiController(IServiceProvider services) : ControllerBase
{
    protected IMediator Mediator { get; set; } = services.GetRequiredService<IMediator>();

    protected ActionResult HandleResult<T>(Result<T>? result)
    {
        if (result == null) {
            return NotFound();
        }

        return result.IsSuccess switch
        {
            true when result.Value != null => this.Ok(new { data = result.Value, }),
            true when result.Value == null => NotFound(),
            _ => BadRequest(result.Error)
        };
    }
}