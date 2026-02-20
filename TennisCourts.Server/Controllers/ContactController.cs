using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace TennisCourts.Server.Controllers
{
    [ApiController]
    [Route("api/contact")]
    public class ContactController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly CaptchaService _captcha;

        public ContactController(EmailService emailService, CaptchaService captcha)
        {
            Console.WriteLine("chliinu");
            Console.WriteLine(Environment.GetEnvironmentVariable("HCaptcha__Secret"));
            _emailService = emailService;
            _captcha = captcha;
        }

        [EnableRateLimiting("formLimiter")]
        [HttpPost]
        public async Task<IActionResult> Send(ContactDto dto)
        {
            if (!await _captcha.VerifyCaptchaAsync(dto.CaptchaToken))
                return BadRequest("Captcha failed");

            bool result = await _emailService.SendAsync(dto);
            return Ok(result);
        }
    }
}
