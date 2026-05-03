using Microsoft.AspNetCore.Mvc;

namespace SystemSaude.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthProfessionalController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "API SystemSaude ativa.",
                documentation = "/swagger"
            });
        }
    }
}
