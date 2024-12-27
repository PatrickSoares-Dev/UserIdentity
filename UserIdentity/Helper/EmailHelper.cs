using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.IO;
public class EmailHelper
{
    private readonly IConfiguration _configuration;
    private readonly string _from;
    private readonly string _host;
    private readonly int _port;
    private readonly string _username;
    private readonly string _password;
    private readonly string _templatePath;
    private readonly string _cc;
    private readonly string _bcc;

    public EmailHelper(IConfiguration configuration)
    {
        _configuration = configuration;
        _from = _configuration["EmailSettings:From"];
        _host = _configuration["EmailSettings:Host"];
        _port = int.Parse(_configuration["EmailSettings:Port"]);
        _username = _configuration["EmailSettings:Username"];
        _password = _configuration["EmailSettings:Password"];
        _templatePath = _configuration["EmailSettings:TemplatePath"];
    }

    public void SendEmail(string to, string subject, string body)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_from),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(to);

        if (!string.IsNullOrEmpty(_cc))
        {
            mailMessage.CC.Add(_cc);
        }

        if (!string.IsNullOrEmpty(_bcc))
        {
            mailMessage.Bcc.Add(_bcc);
        }

        using (var smtpClient = new SmtpClient(_host, _port))
        {
            smtpClient.Credentials = new NetworkCredential(_username, _password);
            smtpClient.EnableSsl = true;
            smtpClient.Send(mailMessage);
        }
    }

    public string GetEmailTemplate()
    {
        return File.ReadAllText(_templatePath);
    }
}
