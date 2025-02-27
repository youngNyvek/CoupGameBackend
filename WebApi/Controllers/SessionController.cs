using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly SessionService _service;

        public SessionController(SessionService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSession()
        {
            var session = await _service.CreateSessionAsync();
            return Ok(new { sessionCode = session.SessionCode });
        }

        [HttpPost("{sessionCode}/increment")]
        public async Task<IActionResult> IncrementCounter(string sessionCode)
        {
            await _service.IncrementCounterAsync(sessionCode);
            return NoContent();
        }

        [HttpPost("{sessionCode}/decrement")]
        public async Task<IActionResult> DecrementCounter(string sessionCode)
        {
            await _service.DecrementCounterAsync(sessionCode);
            return NoContent();
        }
    }
}
