using Microsoft.AspNetCore.Mvc;
using ReadingEmail.Models;
using ReadingEmail.Services;

namespace ReadingEmail.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController(IMailService mailService) : ControllerBase
    {
        private readonly IMailService _mailService = mailService;


        [HttpGet("mails")]
        public IActionResult GetEmails([FromQuery] string? searchTerm, SearchType? type)
        {
            return Ok(_mailService.GetEmails(searchTerm,type));
        }
    }
}
