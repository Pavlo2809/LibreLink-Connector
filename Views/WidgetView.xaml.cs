using System.Windows;
using System.Windows.Controls;
using LibreLinkConnector.Presenters;

namespace LibreLinkConnector.Views
{
    public partial class WidgetView : UserControl
    {
        private readonly ForecastPresenter _presenter;
        public event EventHandler? ExpandRequested;

        public WidgetView(ForecastPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            DataContext = _presenter;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await _presenter.RefreshDataAsync();
        }

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            ExpandRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
