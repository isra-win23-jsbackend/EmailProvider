using System;
using System.Threading.Tasks;
using Azure;
using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using EmailProvider.Models;
using EmailProvider.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EmailProvider.Funtions;

public class EmailSender(ILogger<EmailSender> logger, IEmailService emailService)
{
    private readonly ILogger<EmailSender> _logger = logger;
    private readonly IEmailService _emailService = emailService;

    [Function(nameof(EmailSender))]
    public async Task Run([ServiceBusTrigger("email_request", Connection = "ServiceBusConnection")]ServiceBusReceivedMessage message,ServiceBusMessageActions messageActions)
    {
        try
        {
            var emailRequest = _emailService.UnPackEmailEmailRequest(message);
            if (emailRequest != null && !string.IsNullOrEmpty(emailRequest.To))
            {
                if (_emailService.SendEmail(emailRequest))
                {
                    await messageActions.CompleteMessageAsync(message);
                }
            }
        }
        catch(Exception ex)
        {
            _logger.LogError($"ERROR: EmailSender.Run() :: {ex.Message}");
        }
    }


}


//try
//{

//}
//catch (Exception ex)
//{
//    _logger.LogError($"ERROR: EmailSender.Run() :: {ex.Message}");
//}