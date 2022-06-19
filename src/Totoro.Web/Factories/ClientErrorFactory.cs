using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Totoro.Web.Factories;

public class ClientErrorFactory : IClientErrorFactory
{
    public IActionResult GetClientError(ActionContext context, IClientErrorActionResult error)
    {
        throw new NotImplementedException();
    }
}
