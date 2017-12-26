using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace RevitHP
{
    /// <summary>
    /// LoginForm.xaml 的交互逻辑
    /// </summary>
    public partial class LoginForm : Window
    {
        private FamilyBrowserVM vM;
        public LoginForm(FamilyBrowserVM browserVM)
        {
            InitializeComponent();
            vM = browserVM;
        } 

        //添加一个委托
        public delegate void PassDataBetweenFormHandler(object sender, LoginState e);
        //添加一个PassDataBetweenFormHandler类型的事件
        public event PassDataBetweenFormHandler PassDataBetweenForm;


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.tb_username.Text == "" || this.tb_password.Password == "")
            {
                MessageBox.Show("请输入用户名和密码!");
            }
            else
            {          
                  HttpClientDoPostLogin(tb_username.Text, tb_password.Password);          
            }

        }

        public async void HttpClientDoPostLogin(string Name, string Password)
        {
            bool obj = await vM.isloginAsync(Name,Password);
            if (obj)
            {              
                LoginState args = new LoginState(ServerManagement.roleName);
                PassDataBetweenForm(this, args);
                this.Close();
            }
            else
            {
                MessageBox.Show("登录失败：用户名或者密码不正确");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
