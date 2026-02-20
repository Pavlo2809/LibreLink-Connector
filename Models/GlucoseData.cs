namespace LibreLinkConnector.Models
{
    /// <summary>
    /// Represents current glucose reading and historical data
    /// </summary>
    public class GlucoseData
    {
        // Current reading
        public double CurrentValue { get; set; }
        public int TrendArrow { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public GlucoseUnit Unit { get; set; } = GlucoseUnit.MgPerDl;
        
        // Historical readings
        public List<GlucoseMeasurement> History { get; set; } = new();
        
        // Computed properties
        public string TrendArrowSymbol => TrendArrow switch
        {
            1 => "↑",
            2 => "↑↑",
            3 => "→",
            4 => "↓",
            5 => "↓↓",
            _ => "→"
        };
        
        public double DisplayValue => Unit == GlucoseUnit.MgPerDl ? CurrentValue : CurrentValue / 18.0;
        
        public string UnitText => Unit == GlucoseUnit.MgPerDl ? "mg/dL" : "mmol/L";
        
        public GlucoseStatus GetStatus(double highThreshold, double lowThreshold)
        {
            var value = DisplayValue;
            if (value >= highThreshold)
                return GlucoseStatus.High;
            if (value <= lowThreshold)
                return GlucoseStatus.Low;
            return GlucoseStatus.Normal;
        }
        
        public string GetStatusText(double highThreshold, double lowThreshold)
        {
            return GetStatus(highThreshold, lowThreshold) switch
            {
                GlucoseStatus.High => "HIGH",
                GlucoseStatus.Low => "LOW",
                GlucoseStatus.Normal => "Normal",
                _ => "Unknown"
            };
        }
        
        public void Clear()
        {
            CurrentValue = 0;
            TrendArrow = 3; // Stable
            LastUpdateTime = DateTime.MinValue;
            History.Clear();
        }
    }
    
    public enum GlucoseStatus
    {
        Normal,
        High,
        Low
    }
}
