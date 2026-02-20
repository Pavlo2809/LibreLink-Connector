using Newtonsoft.Json;

namespace LibreLinkConnector.Models
{
    public class LoginRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("password")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("data")]
        public LoginData? Data { get; set; }
    }

    public class LoginData
    {
        [JsonProperty("user")]
        public User? User { get; set; }

        [JsonProperty("authTicket")]
        public AuthTicket? AuthTicket { get; set; }

        [JsonProperty("redirect")]
        public bool Redirect { get; set; }

        [JsonProperty("region")]
        public string? Region { get; set; }
    }

    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonProperty("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("country")]
        public string Country { get; set; } = string.Empty;
    }

    public class AuthTicket
    {
        [JsonProperty("token")]
        public string Token { get; set; } = string.Empty;

        [JsonProperty("expires")]
        public long Expires { get; set; }

        [JsonProperty("duration")]
        public long Duration { get; set; }
    }
}
