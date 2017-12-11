using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net.Http;
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

namespace RevitHP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            else {
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
                string tex = item.Id.ToString();
                MessageBox.Show(tex);
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
                //parent.Children.Remove(item);
                item.Identifying = Convert.ToInt32(CataItem.Stater.Delete);
                MessageBox.Show(item.Name.ToString());
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //LoginForm login = new LoginForm();
            //login.ShowDialog();
            loginAsync();
        }
        FamilyMessage family = new FamilyMessage();
        public async Task loginAsync()
        {
            using (var client = new HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("username","admin"));
                values.Add(new KeyValuePair<string, string>("password", "admin"));
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("http://114.113.234.159:8074/bim/revit/revit/login", content);
                //获取登陆头部信息
                //var Headers = responsea.Headers.ToString();             
                //获得["ACCESS-TOKEN"]
                //MessageBox.Show(((System.String[])response.Headers.GetValues("ACCESS-TOKEN"))[0].ToString());
                //获取登陆后主体信息                           
                family.ACCESS_TOKEN = ((System.String[])response.Headers.GetValues("ACCESS-TOKEN"))[0].ToString();
                var responseString = await response.Content.ReadAsStringAsync();
                //Newtonsoft.Json.Linq.JObject obj =(Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(responseString);
                JObject obj = (JObject)JsonConvert.DeserializeObject(responseString);
                //obj.GetValue("obj")["roles"][0]["id"].ToString()

                MessageBox.Show(obj.GetValue("obj")["roles"][0]["roleName"].ToString());

                //var tagValues = JObject.Parse(obj["success"].ToString()).ToObject<Dictionary<string, List<string>>>();
                //MessageBox.Show(tagValues.ToString());
                //var obj = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(responseString);
                //MessageBox.Show(obj.GetValue("roleName").ToString());

                //MessageBox.Show(responseString.ToString());
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            HttpClientDoPost();
        }

        public async void HttpClientDoPost()
        {
            using (var client = new HttpClient())
            {
                
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("ACCESS-TOKEN",(family.ACCESS_TOKEN)));
                //values.Add(new KeyValuePair<string, string>("password",Password));
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("http://114.113.234.159:8074/bim/revit/revit/logout", content);
                var responseString = await response.Content.ReadAsStringAsync();
                MessageBox.Show(responseString.ToString());
            }
        }

    }
}
