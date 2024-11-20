using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Services.Mail;

public interface IMailSender {
	Task<string> SendOrderConfirmationAsync(TicketOrderMailData data);
}

public interface ISmtpRelay {
	Task<string> SendMailAsync(MimeMessage message);
}

public class SmtpMailSender(IMailBodyRenderer renderer, ISmtpRelay smtpRelay) : IMailSender {

	private readonly MailboxAddress mailFrom = new("Rockaway Tickets", "tickets@rockaway.dev");

	public MimeMessage BuildOrderConfirmationMail(TicketOrderMailData data) {
		var message = new MimeMessage();
		message.Subject = $"Your tickets to {data.Artist.Name} at {data.VenueName}";
		message.From.Add(mailFrom);
		message.To.Add(new MailboxAddress(data.CustomerName, data.CustomerEmail));
		var bb = new BodyBuilder {
			HtmlBody = renderer.RenderOrderConfirmationHtml(data),
			TextBody = renderer.RenderOrderConfirmationText(data)
		};
		message.Body = bb.ToMessageBody();
		return message;
	}

	public async Task<string> SendOrderConfirmationAsync(TicketOrderMailData data) {
		var message = BuildOrderConfirmationMail(data);
		return await smtpRelay.SendMailAsync(message);
	}
}

public class SmtpRelay(SmtpSettings settings, ILogger<SmtpRelay> logger) : ISmtpRelay {
	public async Task<string> SendMailAsync(MimeMessage mail) {
		using var smtp = new SmtpClient();
		await smtp.ConnectAsync(settings.Host, settings.Port);
		if (settings.Authenticate) {
			await smtp.AuthenticateAsync(settings.Username, settings.Password);
		}
		var result = await smtp.SendAsync(mail);
		await smtp.DisconnectAsync(true);
		return result;
	}
}

public class SmtpSettings {
	public string Host { get; set; } = "localhost";
	public string? Username { get; set; }
	public string? Password { get; set; }
	public int Port { get; set; } = 1025;
	public bool Authenticate => !(Username.IsNullOrEmpty() || Password.IsNullOrEmpty());
}