using System.Windows;
using System.Windows.Controls;

namespace RevitHP
{
    /// <summary>
    /// LoginForm.xaml 的交互逻辑
    /// </summary>
    public partial class LoginForm : Window
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnPwdChanged(object sender, RoutedEventArgs e)
        {
            var box = sender as PasswordBox;
            var vm = this.DataContext as LoginVM;
            if (box != null && vm != null) {
                
                vm.Pwd = box.SecurePassword;
            }
        }

        private void OnLogin(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as LoginVM;
            vm.Login();
        }
    }
}
