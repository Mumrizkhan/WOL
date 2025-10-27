using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

namespace WOL.NotificationWorker.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public NotificationService(
        ILogger<NotificationService> logger,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task SendPushNotificationAsync(string userId, string title, string message, Dictionary<string, string>? data = null)
    {
        try
        {
            var firebaseServerKey = _configuration["Firebase:ServerKey"];
            if (string.IsNullOrEmpty(firebaseServerKey))
            {
                _logger.LogWarning("Firebase ServerKey not configured. Skipping push notification.");
                return;
            }

            var payload = new
            {
                to = $"/topics/user_{userId}",
                notification = new
                {
                    title,
                    body = message,
                    sound = "default"
                },
                data = data ?? new Dictionary<string, string>()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"key={firebaseServerKey}");

            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Push notification sent successfully to user {UserId}", userId);
            }
            else
            {
                _logger.LogError("Failed to send push notification. Status: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending push notification to user {UserId}", userId);
        }
    }

    public async Task SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            var twilioAccountSid = _configuration["Twilio:AccountSid"];
            var twilioAuthToken = _configuration["Twilio:AuthToken"];
            var twilioPhoneNumber = _configuration["Twilio:PhoneNumber"];

            if (string.IsNullOrEmpty(twilioAccountSid) || string.IsNullOrEmpty(twilioAuthToken))
            {
                _logger.LogWarning("Twilio credentials not configured. Skipping SMS.");
                return;
            }

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{twilioAccountSid}:{twilioAuthToken}"));
            
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("To", phoneNumber),
                new KeyValuePair<string, string>("From", twilioPhoneNumber ?? ""),
                new KeyValuePair<string, string>("Body", message)
            });

            var request = new HttpRequestMessage(HttpMethod.Post, 
                $"https://api.twilio.com/2010-04-01/Accounts/{twilioAccountSid}/Messages.json")
            {
                Content = content
            };
            request.Headers.Add("Authorization", $"Basic {credentials}");

            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("SMS sent successfully to {PhoneNumber}", phoneNumber);
            }
            else
            {
                _logger.LogError("Failed to send SMS. Status: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS to {PhoneNumber}", phoneNumber);
        }
    }

    public async Task SendEmailAsync(string email, string subject, string body)
    {
        try
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUsername = _configuration["Email:Username"];
            var smtpPassword = _configuration["Email:Password"];
            var fromEmail = _configuration["Email:FromEmail"];
            var fromName = _configuration["Email:FromName"] ?? "World of Logistics";

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername))
            {
                _logger.LogWarning("Email SMTP settings not configured. Skipping email.");
                return;
            }

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail ?? smtpUsername, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Email}", email);
        }
    }
}
