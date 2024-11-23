using System.Text.Json;
using System.Text.Json.Serialization;

namespace EmailCollector.Api.Services.EmailValidations;

public static class RecaptchaValidation
{
    private const string RecaptchaVerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

    public static async Task<bool> ValidateAsync(string token, string secretKey)
    {
        using var httpClient = new HttpClient();

        var response = await httpClient.PostAsync(RecaptchaVerifyUrl, 
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "secret", secretKey },
                { "response", token }
            }));

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RecaptchaResponse>(jsonResponse);

        return result?.Success == true;
    }

    private class RecaptchaResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error-codes")]
        public string[] ErrorCodes { get; set; }
    }
}
