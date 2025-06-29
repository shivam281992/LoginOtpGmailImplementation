using LoginOtpGmailImplementation.Data;
using LoginOtpGmailImplementation.Models;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Collections.Concurrent;


namespace LoginOtpGmailImplementation.Services
{
    public class OtpService: IOtpService
    {
        
        private readonly TimeSpan _otpExpiry = TimeSpan.FromMinutes(1);
        private readonly AppDbContext _context;
        public OtpService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DateTime> SendOtpAsync(string email)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            var generatedAt = DateTime.UtcNow;

            var entry = new OtpEntry
            {
                Email = email,
                Otp = otp,
                GeneratedAt = generatedAt,
                IsUsed = false
            };

            _context.OtpEntries.Add(entry);
            await _context.SaveChangesAsync();

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse("shivam.muskaan.kumar@gmail.com"));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Your OTP Code";
            message.Body = new TextPart("html")
            {
                Text = $@"
        <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Your One-Time Password (OTP)</h2>
                <p><strong>{otp}</strong></p>
                <p>This OTP is valid until <strong>{generatedAt.Add(_otpExpiry):HH:mm:ss} UTC</strong>.</p>
                <p>If you did not request this code, please ignore this email.</p>
                <br />
                <p>Thank you,<br/>Shivam Team</p>
            </body>
        </html>"
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("shivam.muskaan.kumar@gmail.com", "rldgcqpmzpomqlht"); // App password, not your actual Gmail password
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);

            return generatedAt.Add(_otpExpiry);
        }
        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            var now = DateTime.UtcNow;

            var entry = await _context.OtpEntries
                .Where(e => e.Email == email && e.Otp == otp && !e.IsUsed)
                .OrderByDescending(e => e.GeneratedAt)
                .FirstOrDefaultAsync();

            if (entry == null) return false;

            if (now - entry.GeneratedAt > _otpExpiry)
                return false;

            entry.IsUsed = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
    
}
