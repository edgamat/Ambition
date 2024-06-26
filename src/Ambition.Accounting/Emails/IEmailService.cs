﻿
namespace Ambition.Accounting.Emails
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string body, CancellationToken cancellationToken);
    }
}