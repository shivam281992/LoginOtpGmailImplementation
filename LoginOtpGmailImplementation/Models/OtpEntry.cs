namespace LoginOtpGmailImplementation.Models
{
    public class OtpEntry
    {
        public int Id { get; set; }

        public string Email { get; set; }=string.Empty;

        public string Otp { get; set; }=string.Empty;

        public DateTime GeneratedAt { get; set; }

        public bool IsUsed { get; set; }

    }
}
