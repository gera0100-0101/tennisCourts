namespace TennisCourts.Server
{
    using System.Net.Http;
    using System.Net.Http.Json;

    public class CaptchaService
    {
        private readonly HttpClient _http;
        private readonly string _secret;

        public CaptchaService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _secret = config["HCaptcha:Secret"];
        }

        public async Task<bool> VerifyCaptchaAsync(string token)
        {
            var response = await _http.PostAsync(
                "https://hcaptcha.com/siteverify",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                { "secret", _secret },
                { "response", token }
                })
            );

            var result = await response.Content.ReadFromJsonAsync<CaptchaResponse>();
            return result?.Success == true;
        }

    }
}
