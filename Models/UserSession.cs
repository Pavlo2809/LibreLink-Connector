namespace LibreLinkConnector.Models
{
    /// <summary>
    /// Represents the current user session with authentication and patient data
    /// </summary>
    public class UserSession
    {
        public string Email { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime TokenExpiry { get; set; }
        public bool IsAuthenticated => !string.IsNullOrEmpty(AuthToken) && DateTime.Now < TokenExpiry;
        
        // Current patient information
        public string? PatientId { get; set; }
        public string? PatientFirstName { get; set; }
        public string? PatientLastName { get; set; }
        public string PatientFullName => !string.IsNullOrEmpty(PatientFirstName) && !string.IsNullOrEmpty(PatientLastName) 
            ? $"{PatientFirstName} {PatientLastName}" 
            : "--";
        
        // Patient target ranges
        public int TargetHigh { get; set; } = 180;
        public int TargetLow { get; set; } = 70;
        
        public void Clear()
        {
            Email = string.Empty;
            AuthToken = string.Empty;
            UserId = string.Empty;
            TokenExpiry = DateTime.MinValue;
            PatientId = null;
            PatientFirstName = null;
            PatientLastName = null;
            TargetHigh = 180;
            TargetLow = 70;
        }
    }
}
