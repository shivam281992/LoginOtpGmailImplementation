using LoginOtpGmailImplementation.Models;
using LoginOtpGmailImplementation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoginOtpGmailImplementation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IOtpService _otpService;

        public AuthController(IOtpService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpRequest request)
        {
            var expiry = await _otpService.SendOtpAsync(request.Email);
            return Ok(new
            {
                message = "OTP sent successfully.",
                expiresAt = expiry
            });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyRequest request)
        {
            var isValid = await _otpService.VerifyOtpAsync(request.Email, request.Otp);
            if (!isValid)
                return Unauthorized("Invalid or expired OTP");

            return Ok("OTP verified. Logged in.");
        }
    }
}
