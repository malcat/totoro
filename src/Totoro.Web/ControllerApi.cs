using Microsoft.AspNetCore.Mvc;

namespace Totoro.Web;

[ApiController]
[Route("v{version:apiVersion}/[controller]")]
public abstract class ControllerApi : ControllerBase
{
    public new OkObjectResult Ok()
    {
        return base.Ok(new { });
    }
}
