using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymWebApp.WebAPI.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }
}
