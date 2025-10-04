using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotificationMicroservice.Application.Dtos;
using NotificationMicroservice.Application.Interfaces;

namespace NotificationMicroservice.API.Controllers
{
    [Route("api/[controller]")]
    public class NotificationController(
        IEmailService emailService)
        : ControllerBase
    {
        [HttpPost("send-email")]
        public async Task<ActionResult<EmailResponse>> SendEmail([FromBody] EmailRequest emailDto)
        {
            var result = await emailService.SendEmailAsync(emailDto);

            if (result.Success)
            {
                return Ok(result);
            }
            return result.Message.Contains("Unauthorized")
                ? Unauthorized(result)
                : BadRequest(result);
            
        }

        [HttpGet("status/{emailId}")]
        public async Task<ActionResult<EmailResponse>> GetEmailStatus(string emailId)
        {
            var result = await emailService.GetEmailStatusAsync(emailId);

            if (result.Success || result.Message == "Email notification not found")
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}