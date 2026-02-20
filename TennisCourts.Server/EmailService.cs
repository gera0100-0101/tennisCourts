using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Org.BouncyCastle.Crypto;
using System.Text.RegularExpressions;

namespace TennisCourts.Server
{
    public class EmailService
    {
        private readonly string _emailSend;
        private readonly string _emailAccept;
        private readonly string _password;

        public EmailService(IConfiguration config)
        {
            _emailSend = config["SMTP:EmailSend"];
            _emailAccept = config["SMTP:EmailAccept"];
            _password = config["SMTP:Pass"];
        }

        public async Task<bool> SendAsync(ContactDto dto)
        {
            Regex mailRegex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
            Match mailMatch = mailRegex.Match(dto.Email);
            Regex phoneRegex = new Regex(@"^\+?\d{1,3}?[\s-]?\(?\d{3}\)?[\s-]?\d{3}[\s-]?\d{2}[\s-]?\d{2}$");
            Match phoneMatch = phoneRegex.Match(dto.Phone);

            if (!mailMatch.Success || !phoneMatch.Success)
            {
                return false;
            }

            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Site", _emailSend));
            message.To.Add(new MailboxAddress("You", _emailAccept));
            message.Subject = "Заявка с сайта Padel Courts";
            string fullMessage = $"Отправитель: {dto.Name}\nПочта: {dto.Email}\nТелефон: {dto.Phone}\nСообщение: {dto.Message}";
            message.Body = new TextPart("plain") { Text = fullMessage };

            using SmtpClient client = new SmtpClient();
            await client.ConnectAsync("smtp.yandex.ru", 465, true);
            await client.AuthenticateAsync(_emailSend, _password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            return true;
        }
    }
}
