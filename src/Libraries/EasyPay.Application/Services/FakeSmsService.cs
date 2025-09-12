using EasyPay.Application.Services;
using Microsoft.Extensions.Logging;

public class FakeSmsService : ISmsService
{
    private readonly ILogger<FakeSmsService> _logger;

    public FakeSmsService(ILogger<FakeSmsService> logger)
    {
        _logger = logger;
    }

    public Task SendVerificationCodeAsync(string phoneNumber, string code)
    {
        _logger.LogInformation("--- SIMULATING SMS ---");
        _logger.LogInformation("To: {PhoneNumber}", phoneNumber);
        _logger.LogInformation("Verification Code: {Code}", code);
        _logger.LogInformation("--- END SMS SIMULATION ---");

        return Task.CompletedTask;
    }
}