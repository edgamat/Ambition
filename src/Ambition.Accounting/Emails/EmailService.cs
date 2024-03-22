using System.Text;
using System.Text.Json;

namespace Ambition.Accounting.Emails;

public class EmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmailService> _logger;

    public EmailService(HttpClient httpClient, ILogger<EmailService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string body)
    {
        _logger.LogInformation("Sending email to {Email} with subject {Subject}", email, subject);

        _httpClient.BaseAddress = new Uri("https://localhost:7191/");

        using StringContent jsonContent = new(
                JsonSerializer.Serialize(new { email, subject, body }),
                Encoding.UTF8,
                "application/json");

        using HttpResponseMessage response = await _httpClient.PostAsync("send", jsonContent);

        response.EnsureSuccessStatusCode();
    }
}
