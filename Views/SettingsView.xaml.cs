using System.Windows;
using System.Windows.Controls;
using LibreLinkConnector.Presenters;

namespace LibreLinkConnector.Views
{
    public partial class SettingsView : UserControl
    {
        private readonly SettingsPresenter _presenter;
        public event EventHandler? SaveRequested;
        public event EventHandler? CancelRequested;

        public SettingsView(SettingsPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            DataContext = _presenter;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_presenter.ValidateAndSave())
            {
                SaveRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _presenter.LoadSettings(); // Revert changes
            CancelRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
