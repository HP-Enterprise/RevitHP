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
        public LoginForm()
        {
            InitializeComponent();
        }
        //实例化
        ServeManagement Serve = new ServeManagement();
        

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
                HttpClientDoPost(this.tb_username.Text, this.tb_password.Password);
            }

        }
        //声明公开静态ACCESS_TOKEN参数
        public static string family_ACCESS_TOKEN;

        public async void HttpClientDoPost(string Name, string Password)
        {

            //using (var client = new HttpClient())
            //{
            //    var values = new List<KeyValuePair<string, string>>();
            //    values.Add(new KeyValuePair<string, string>("username", "admin"));
            //    values.Add(new KeyValuePair<string, string>("password", "admin"));
            //    var content = new FormUrlEncodedContent(values);
            //    //var response = await client.PostAsync("http://192.168.10.54:9007/revit/login",content);


            //    var response = await client.PostAsync("http://114.113.234.159:8074/bim/revit/revit/login", content);
            //    //获取登陆头部信息
            //    //var Headers = responsea.Headers.ToString();             
            //    //获得["ACCESS-TOKEN"]
            //    //((System.String[])response.Headers.GetValues("ACCESS-TOKEN"))[0].ToString();
            //    //获取登陆后主体信息
            //    //var responseString = await responsea.Content.ReadAsStringAsync();             
            //    var responseString = await response.Content.ReadAsStringAsync();
            //    JObject obj = (JObject)JsonConvert.DeserializeObject(responseString);
            //    string islogin = obj.GetValue("success").ToString();
            //    if (islogin == "True")
            //    {
            //        string roleName = obj.GetValue("obj")["roles"][0]["roleName"].ToString();
            //        family_ACCESS_TOKEN = ((System.String[])response.Headers.GetValues("ACCESS-TOKEN"))[0].ToString();
            //        LoginState args = new LoginState(roleName);
            //        PassDataBetweenForm(this, args);
            //        this.Close();
            //    }
            //    else
            //    {
            //        MessageBox.Show("登录失败：用户名或者密码不正确");
            //    }
            //}
            var obj= await Serve.HttpClientDoPostAsync("admin", "admin");
            string islogin = obj.GetValue("success").ToString();
            if (islogin == "True")
            {
                string roleName = obj.GetValue("obj")["roles"][0]["roleName"].ToString();              
                LoginState args = new LoginState(roleName);
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
