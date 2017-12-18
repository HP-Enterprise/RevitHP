using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RevitHP
{
   
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        const string UploadURL = " http://192.168.10.54:9007/revit/file/upload";
        public MainWindow()
        {
            InitializeComponent();
            //测试查询数据
            //RevitBiz biz = new RevitBiz();
            //biz.init();          
            //this.textbox.Text = biz.FindBing();

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 如果是嵌入在Revit中,只隐匿
            // 如果是以主窗口启动,直接关闭
            if (Application.Current == null)
            {
                // 这是由Revit启动加载插件,没有Application对象
                // 只隐匿，不真正关闭窗口
                // Revit主程序退出时，附带自动关闭所有窗口
                Hide();
                e.Cancel = true;
            }
            else
            {
                // 由Application启动，正常关闭
                e.Cancel = false;
            }
        }

        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CataItem item = Treeview1.SelectedItem as CataItem;
            if (item != null)
            {
                string tex = item.Id.ToString();
                MessageBox.Show(tex);
            }
        }

        private void Datagrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ////两种方式获取ID
            ////第一种
            //DataRowView mySelectedElement = (DataRowView)Datagrid1.SelectedItem;
            //string result = mySelectedElement.Row[0].ToString();

            ////第二种
            ////var Txt = Datagrid1.SelectedItem as DataRowView;
            ////string result = Txt.Row[0].ToString();
            //MessageBox.Show(result);
        }

        private void Treeview1_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //CataItem item = Treeview1.SelectedItem as CataItem;          
            //if (item != null)
            //{
            //    TreeIndex index = new TreeIndex(item);
            //    index.ShowDialog();
            //    string tex = item.ParentID.ToString();
            //    MessageBox.Show(tex);
            //}

        }

        private void InputNode_Click(object sender, RoutedEventArgs e)
        {
            CataItem item = Treeview1.SelectedItem as CataItem;
            if (item != null)
            {
                TreeIndex index = new TreeIndex(item);
                index.ShowDialog();

            }
            else
            {
                MessageBox.Show("请选择父级节点");
            }
        }

        private void DeleteNode_Click(object sender, RoutedEventArgs e)
        {
            CataItem item = Treeview1.SelectedItem as CataItem;

            //((RevitHP.FamilyBrowserVM)Treeview1.DataContext).TreeViewBinding[0].Children[0].children.Add(list);
            if (item == null)
            {
                MessageBox.Show("请选择要删除的节点");
            }
            //获取当前选中节点的父类
            CataItem parent = item.Parent as CataItem;
            if (parent != null)
            {
                //在父节点中删除选中的子节点
                parent.Children.Remove(item);
                item.Identifying = Convert.ToInt32(CataItem.Stater.Delete);

            }
            else
            {
                MessageBox.Show("父节点不能删除");
            }
        }

        private void UpdateNode_Click(object sender, RoutedEventArgs e)
        {
            CataItem item = Treeview1.SelectedItem as CataItem;
            MessageBox.Show(item.Identifying.ToString());
            if (item != null)
            {
                TreeUpdate update = new TreeUpdate(item);
                update.ShowDialog();
            }
        }
        string roleName;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.login_state.Header.ToString() == "登录")
            {
                LoginForm login = new LoginForm();
                login.PassDataBetweenForm += new LoginForm.PassDataBetweenFormHandler(FrmChild_PassDataBetweenForm);
                login.ShowDialog();


            }
            else if (this.login_state.Header.ToString() == "注销")
            {
                if (LoginForm.family_ACCESS_TOKEN != null)
                {
                    HttpClientDoPost(LoginForm.family_ACCESS_TOKEN);
                }
                else
                {
                    MessageBox.Show("您还未登录！");
                }
            }
            //loginAsync();
        }

        public async Task loginAsync()
        {
            using (var client = new HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("username", "admin"));
                values.Add(new KeyValuePair<string, string>("password", "admin"));
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("http://114.113.234.159:8074/bim/revit/revit/login", content);
                //获取登陆头部信息
                //var Headers = responsea.Headers.ToString();             
                //获得["ACCESS-TOKEN"]
                //MessageBox.Show(((System.String[])response.Headers.GetValues("ACCESS-TOKEN"))[0].ToString());
                //获取登陆后主体信息                           
                ((System.String[])response.Headers.GetValues("ACCESS-TOKEN"))[0].ToString();
                var responseString = await response.Content.ReadAsStringAsync();
                JObject obj = (JObject)JsonConvert.DeserializeObject(responseString);
                roleName = obj.GetValue("obj")["roles"][0]["roleName"].ToString();
                this.welcome.Content = "欢迎" + roleName + "进入此系统";
            }
        }
        private void FrmChild_PassDataBetweenForm(object sender, LoginState e)
        {
            this.welcome.Content = e.RoleName;
            this.login_state.Header = "注销";
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           
            if (LoginForm.family_ACCESS_TOKEN != null && this.welcome.Content.ToString() == "未登录")
            {
                HttpClientDoPost(LoginForm.family_ACCESS_TOKEN);
            }
            else
            {
                MessageBox.Show("您还未登录！");
            }

        }

        //注销
        public async void HttpClientDoPost(string ACCESS_TOKEN)
        {
            using (var client = new HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("ACCESS-TOKEN", ACCESS_TOKEN));
                //values.Add(new KeyValuePair<string, string>("password",Password));
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("http://114.113.234.159:8074/bim/revit/revit/logout", content);
                var responseString = await response.Content.ReadAsStringAsync();
                JObject obj = (JObject)JsonConvert.DeserializeObject(responseString);
                if (obj.HasValues)
                {
                    this.login_state.Header = "登录";
                    this.welcome.Content = "未登录";
                    MessageBox.Show("注销成功");
                }
            }
        }

        private void Ceshi_Click(object sender, RoutedEventArgs e)
        {
          
        }

        //上传
        private void clanuploading_Click(object sender, RoutedEventArgs e)
        {
            clanUploadingInfoAsync(LoginForm.family_ACCESS_TOKEN);
        }
        RevitBiz path = new RevitBiz();
        //上传方法
        private async void clanUploadingInfoAsync(string ACCESS_TOKEN)
        {
            //    var client = new RestClient("http://192.168.10.54:9007/revit/file/upload");
            //    var request = new RestRequest(Method.POST);
            //    request.AddHeader("Postman-Token", "7dac66d2-ac51-8d05-e22a-87ea270c8451");
            //    request.AddHeader("Cache-Control", "no-cache");
            //    request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
            //    request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", "------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"ACCESS-TOKEN\"\r\n\r\nBIM eyJhbGciOiJIUzUxMiJ9.eyJBQ0NFU1MtVE9LRU4iOnsidXNlcklkIjoxLCJ0b2tlbklkIjoiYmIyNWQ5ODctZjk0My00OGExLWFlOWEtOWE5ZjI1OTliOWQyIn0sImV4cCI6MTUxMzU2NDE5NH0.3S1S_dy6TdcYPCMGGQ6LWlMsluE6nF5cDIiv4DMhjn4cKnrhe89jiHwyfrcsOpCWfVsQT88NtZ6l702B42ZAuQ\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"file\"; filename=\"F:\\ceshi.txt\"\r\nContent-Type: text/plain\r\n\r\n\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--", ParameterType.RequestBody);
            //    //IRestResponse response = client.Execute(request);
            //    var response = client.Execute(request);
            //    JObject obj = (JObject)JsonConvert.DeserializeObject(response.Content.ToString());

            string str = System.Text.Encoding.Default.GetString(ConvertToBinary(path.sqlLitepath));
            using (var client = new HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("ACCESS-TOKEN", ACCESS_TOKEN));
                values.Add(new KeyValuePair<string, byte>("file", str));
                //values.Add(new KeyValuePair<string, string>("password",Password));
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("http://192.168.10.54:9007/revit/file/upload", content);
                var responseString = await response.Content.ReadAsStringAsync();
                JObject obj = (JObject)JsonConvert.DeserializeObject(responseString);

            }

        }
    

        public static byte[] ConvertToBinary(string Path)
        {
            using (FileStream fs = File.OpenRead(@"F:\\ceshi.txt"))
            { 
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, Convert.ToInt32(fs.Length));
                return buffer;
            }
            
           
        }




        //下载事件
        private void download_Click(object sender, RoutedEventArgs e)
        {
            DownloadAsync(LoginForm.family_ACCESS_TOKEN);
        }
        //下载方法
        private async Task DownloadAsync(string ACCESS_TOKEN)
        {
            using (var client = new HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("ACCESS-TOKEN", ACCESS_TOKEN));
                //values.Add(new KeyValuePair<string, string>("password",Password));
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("http://192.168.10.54:9007/revit/file/download/31", content);  
               
            }
        }

        /// <summary>
        /// 使用HttpClient上传文件及传递参数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postParameters"></param>
        /// <param name="lstFilePath"></param>
        /// <returns></returns>
        public static async Task<string> HttpPostAsync(string url, NameValueCollection postParameters, List<string> lstFilePath= null)
        {
            //var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

            using (var httpClient = new HttpClient())
            {
                using (MultipartFormDataContent formData = new MultipartFormDataContent())
                {
                    if (lstFilePath != null)
                    {
                        for (int i = 0; i < lstFilePath.Count; i++)
                        {
                            int start = lstFilePath[i].LastIndexOf('\\');
                            string name = lstFilePath[i].Substring(start + 1);
                            var fileContent = new ByteArrayContent(File.ReadAllBytes(lstFilePath[i]));
                            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("file")
                            {
                                FileName = name
                            };
                            formData.Add(fileContent);
                        }
                    }
                    foreach (string key in postParameters.Keys)
                    {
                        formData.Add(new StringContent(postParameters[key]), key);
                        
                    }

                    Task<HttpResponseMessage> response = httpClient.PostAsync(url, formData);
                    string tempResult = await response.Result.Content.ReadAsStringAsync();
                    return tempResult;
                }
            }
        }

        private void CeshiLogin_Click(object sender, RoutedEventArgs e)
        {
            loginAsync();
        }
    }
}
