using Newtonsoft.Json;

namespace LibreLinkConnector.Models
{
    public class ConnectionsResponse
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("data")]
        public List<Connection>? Data { get; set; }

        [JsonProperty("ticket")]
        public AuthTicket? Ticket { get; set; }
    }

    public class Connection
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("patientId")]
        public string PatientId { get; set; } = string.Empty;

        [JsonProperty("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonProperty("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonProperty("country")]
        public string Country { get; set; } = string.Empty;

        [JsonProperty("targetLow")]
        public int TargetLow { get; set; }

        [JsonProperty("targetHigh")]
        public int TargetHigh { get; set; }

        [JsonProperty("glucoseMeasurement")]
        public GlucoseMeasurement? GlucoseMeasurement { get; set; }
    }
}
