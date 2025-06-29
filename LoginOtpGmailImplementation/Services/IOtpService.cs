using System.Threading.Tasks;

namespace LoginOtpGmailImplementation.Services
{
    public interface IOtpService
    {
        Task<DateTime> SendOtpAsync(string email);

        Task<bool> VerifyOtpAsync(string email, string otp);
    }
}
