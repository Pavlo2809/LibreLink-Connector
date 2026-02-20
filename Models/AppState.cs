namespace LibreLinkConnector.Models
{
    /// <summary>
    /// Represents the overall application state
    /// </summary>
    public class AppState
    {
        public UserSession Session { get; } = new();
        public GlucoseData GlucoseData { get; } = new();
        
        public ViewMode CurrentView { get; set; } = ViewMode.Login;
        public bool IsWidgetMode { get; set; }
        
        public void ClearSession()
        {
            Session.Clear();
            GlucoseData.Clear();
            CurrentView = ViewMode.Login;
        }
    }
    
    public enum ViewMode
    {
        Login,
        Forecast
    }
}
