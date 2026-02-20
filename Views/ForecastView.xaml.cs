using System.Windows;
using System.Windows.Controls;
using LibreLinkConnector.Presenters;

namespace LibreLinkConnector.Views
{
    public partial class ForecastView : UserControl
    {
        private readonly ForecastPresenter _presenter;

        public ForecastView(ForecastPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            DataContext = _presenter;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await _presenter.RefreshDataAsync();
        }
    }
}
