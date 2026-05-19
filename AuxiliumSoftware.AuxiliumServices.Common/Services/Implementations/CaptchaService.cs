using AuxiliumSoftware.AuxiliumServices.Common.Configuration;
using AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects;
using AuxiliumSoftware.AuxiliumServices.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services.Implementations
{
    public class CaptchaService : ICaptchaService
    {
        private readonly ConfigurationStructure _configuration;
        private readonly ILogger<CaptchaService> _logger;
        private readonly HttpClient _httpClient;

        public CaptchaService(
            IConfiguration configuration,
            ILogger<CaptchaService> logger,
            HttpClient httpClient
            )
        {
            _configuration = new ConfigurationStructure();
            configuration.Bind(_configuration);

            _logger = logger;

            _httpClient = httpClient;
        }

        public async Task<bool> VerifyRecaptchaAsync(string token, string? clientIp)
        {
            try
            {
                // WARNING WARNING WARNING WARNING WARNING
                if (!_configuration.ReCAPTCHA.UseReCAPTCHA)
                {
                    _logger.LogWarning("reCAPTCHA verification is disabled");
                    return true;
                }
                // WARNING WARNING WARNING WARNING WARNING

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("secret", _configuration.ReCAPTCHA.SecretKey),
                    new KeyValuePair<string, string>("response", token),
                    new KeyValuePair<string, string>("remoteip", clientIp ?? "")
                });

                var response = await _httpClient.PostAsync(
                    "https://www.google.com/recaptcha/api/siteverify",
                    content
                );

                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadFromJsonAsync<RecaptchaResponseDTO>();

                if (jsonResponse == null)
                {
                    _logger.LogError("Failed to parse reCAPTCHA response");
                    return false;
                }

                if (!jsonResponse.Success)
                {
                    _logger.LogWarning(
                        "reCAPTCHA verification failed. Errors: {Errors}",
                        string.Join(", ", jsonResponse.ErrorCodes ?? Array.Empty<string>())
                    );
                    return false;
                }

                if (jsonResponse.Score.HasValue && jsonResponse.Score < _configuration.ReCAPTCHA.ScoreThreshold)
                {
                    _logger.LogWarning(
                        "reCAPTCHA score too low: {Score}",
                        jsonResponse.Score
                    );
                    return false;
                }

                _logger.LogInformation("reCAPTCHA verification successful");
                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to contact reCAPTCHA service");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during reCAPTCHA verification");
                return false;
            }
        }
    }
}
