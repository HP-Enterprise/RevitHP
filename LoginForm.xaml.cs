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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HttpClientDoPost(this.tb_username.Text, this.tb_password.Password);
          
        }
        public async void HttpClientDoPost(string Name,string Password)
        {
            using (var client = new HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("username", Name));
                values.Add(new KeyValuePair<string, string>("password", Password));             
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("http://114.113.234.159:8074/bim/revit/revit/login", content);
                //获取登陆头部信息
                //var Headers = responsea.Headers.ToString();             
                //获得["ACCESS-TOKEN"]
                MessageBox.Show(((System.String[])response.Headers.GetValues("ACCESS-TOKEN"))[0].ToString());
                //获取登陆后主体信息
                //var responseString = await responsea.Content.ReadAsStringAsync();
                FamilyMessage family = new FamilyMessage();
                family.ACCESS_TOKEN = ((System.String[])response.Headers.GetValues("ACCESS-TOKEN"))[0].ToString();
            }

           
        }

    }
}
