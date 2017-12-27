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
        FamilyBrowserVM vM = new FamilyBrowserVM();

        public MainWindow()
        {
            InitializeComponent();
          
        }
        public static bool LoginState=true;

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

        //添加节点事件
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
        //删除节点
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
        //修改节点事件
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

        //委托监控
        private void FrmChild_PassDataBetweenForm(object sender, LoginState e)
        {
            this.welcome.Content = e.RoleName;
            this.login_state.Text = "注销";
        }
       
        //登陆注销事件
        private void login_Click(object sender, RoutedEventArgs e)
        {
            if (LoginState)
            {
                LoginForm login = new LoginForm(vM);
                login.PassDataBetweenForm += new LoginForm.PassDataBetweenFormHandler(FrmChild_PassDataBetweenForm);
                login.ShowDialog();
            }
            else 
            {
                logoutAsync();
            }

        }

        public void logoutAsync()
        {          
            var islogout = Task.Run(vM.islogout);         
            islogout.Wait();
            if (islogout.Result)
            {
                LoginState = true;
                this.welcome.Content = "未登录";
                this.login_state.Text = "登录";
            }
        }
        
        static List<string> SqllitePathList = new List<string>();
        //上传事件
        private void uploading_Click(object sender, RoutedEventArgs e)
        {
            if (this.welcome.Content.ToString() == "未登录")
            {
                MessageBox.Show("请登录再上传文件！");
            }
            else
            {
                RevitBiz biz = new RevitBiz();
                string[] paths = Directory.GetFiles(biz.sqlLitepath);
                foreach (var item in paths)
                {
                    //获取文件后缀名  
                    string extension = System.IO.Path.GetExtension(item).ToLower();
                    SqllitePathList.Add(item);
                }
                //调用上传方法
                //serve.ClanUploadingInfo(ServerManagement.family_ACCESS_TOKEN, SqllitePathList[0]);
            }
        }

        private void download_Click_1(object sender, RoutedEventArgs e)
        {
            //调用下载方法
            //serve.DownloadAsync(ServerManagement.family_ACCESS_TOKEN);
        }

        private void MD5_Click(object sender, RoutedEventArgs e)
        {
            

          
        }
        



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in Treeview1.Items)
            {
                DependencyObject dObject = Treeview1.ItemContainerGenerator.ContainerFromItem(item);              
                ((TreeViewItem)dObject).IsExpanded = true;
            }
        }
    }
}
