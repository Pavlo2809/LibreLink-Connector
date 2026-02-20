using Newtonsoft.Json;

namespace LibreLinkConnector.Models
{
    public class GraphResponse
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("data")]
        public GraphData? Data { get; set; }

        [JsonProperty("ticket")]
        public AuthTicket? Ticket { get; set; }
    }

    public class GraphData
    {
        [JsonProperty("connection")]
        public Connection? Connection { get; set; }

        [JsonProperty("graphData")]
        public List<GlucoseMeasurement>? GraphDataPoints { get; set; }
    }

    public class GlucoseMeasurement
    {
        [JsonProperty("FactoryTimestamp")]
        public string FactoryTimestamp { get; set; } = string.Empty;

        [JsonProperty("Timestamp")]
        public string Timestamp { get; set; } = string.Empty;

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("ValueInMgPerDl")]
        public int ValueInMgPerDl { get; set; }

        [JsonProperty("TrendArrow")]
        public int? TrendArrow { get; set; }

        [JsonProperty("TrendMessage")]
        public string? TrendMessage { get; set; }

        [JsonProperty("MeasurementColor")]
        public int MeasurementColor { get; set; }

        [JsonProperty("GlucoseUnits")]
        public int GlucoseUnits { get; set; }

        [JsonProperty("Value")]
        public double Value { get; set; }

        [JsonProperty("isHigh")]
        public bool IsHigh { get; set; }

        [JsonProperty("isLow")]
        public bool IsLow { get; set; }

        public string TrendArrowSymbol
        {
            get
            {
                return TrendArrow switch
                {
                    1 => "↑↑", // Rising rapidly
                    2 => "↑",  // Rising
                    3 => "→",  // Stable
                    4 => "↓",  // Falling
                    5 => "↓↓", // Falling rapidly
                    _ => "?"
                };
            }
        }

        public string StatusColor
        {
            get
            {
                if (IsHigh) return "#F44336"; // Red
                if (IsLow) return "#F44336";  // Red
                return "#4CAF50"; // Green
            }
        }
    }
}
