using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LibreLinkConnector.Presenters;

namespace LibreLinkConnector.Views
{
    public partial class LoginView : UserControl
    {
        private readonly LoginPresenter _presenter;

        public LoginView(LoginPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            DataContext = _presenter;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await _presenter.PerformLoginAsync(PasswordBox.Password);
        }

        private async void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await _presenter.PerformLoginAsync(PasswordBox.Password);
            }
        }
    }
}
