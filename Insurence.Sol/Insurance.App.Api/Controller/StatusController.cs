using Microsoft.AspNetCore.Mvc;

namespace Insurance.App.Api.Controller;

[ApiController]
[Route("[controller]")]
public class StatusController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Status = "Bot is running" });
    }
}