
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Autodesk.Revit.UI;
//using Autodesk.Revit.DB;

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
        }
        public static bool isLoginState = true;
        private static string md5;
        private static string name;
        //private static string classname;
        //private static int catalogid;
        private static string modelname;
        private static string modelsize;
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
            //    //    TreeIndex index = new TreeIndex(item);
            //    //    index.ShowDialog();
            //    string tex = item.Audit.ToString();
            //    MessageBox.Show(tex);
            //}


            //item.Background = System.Windows.Media.Brushes.CornflowerBlue;

        }

        //添加节点事件
        private void InputNode_Click(object sender, RoutedEventArgs e)
        {
            CataItem item = Treeview1.SelectedItem as CataItem;
            FamilyBrowserVM vm = this.DataContext as FamilyBrowserVM;
            if (item != null)
            {
                TreeIndex index = new TreeIndex(item, vm);
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
           
            FamilyBrowserVM vm = this.DataContext as FamilyBrowserVM;
            if (item == null)
            {
                MessageBox.Show("请选择要删除的节点");
            }
          
            //获取当前选中节点的父类
            CataItem parent = item.Parent as CataItem;
            if (parent != null)
            {
                if (item.NameId==ServerManagement.id||ServerManagement.id==1)
                {
                //在父节点中删除选中的子节点
                vm.SetCatalogdelete(item.Id);
                parent.Children.Remove(item);
                item.Identifying = Convert.ToInt32(CataItem.Stater.Delete);
                }
                else
                {
                    MessageBox.Show("您没有权限删除这个节点");
                }
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
            FamilyBrowserVM vm = this.DataContext as FamilyBrowserVM;
            if (item != null)
            {
                TreeUpdate update = new TreeUpdate(item, vm);
                update.ShowDialog();
            }
        }


        //登陆注销事件
        private void login_Click(object sender, RoutedEventArgs e)
        {
            if (isLoginState)
            {
                LoginForm login = new LoginForm();
                FamilyBrowserVM vm = this.DataContext as FamilyBrowserVM;
                if (login.ShowDialog() == true)
                {
                    this.welcome.Content = ServerManagement.roleName;
                    this.login_state.Text = "注销";
                    if (ServerManagement.id == 1)
                    {
                        audit.Visibility = Visibility.Visible;
                        AuditRefuse.Visibility = Visibility.Visible;
                        DeleteNode.Visibility = Visibility.Visible;
                    }
                    //vm.IsDownload();
                    vm.Downloadnew();
                    UnfoldTreeview();
                    var vM = this.DataContext as FamilyBrowserVM;
                    this.dataGrid.ItemsSource = vM.Modellist();
                }
            }
            else
            {
                logoutAsync();
                UnfoldTreeview();
            }

        }
        public void logoutAsync()
        {
            var vm = this.DataContext as FamilyBrowserVM;
            var islogout = Task.Run(vm.islogout);
            islogout.Wait();
            if (islogout.Result)
            {

                isLoginState = true;
                this.audit.Visibility = Visibility.Collapsed;
                this.AuditRefuse.Visibility = Visibility.Collapsed;
                this.dataGrid.ItemsSource = null;
                this.welcome.Content = "未登录";
                this.login_state.Text = "登录";
                ServerManagement.id = 0;
                vm.DeleteFile2();
                vm.renovation();
            }
        }


        //上传事件      
        private void Uploading_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoginState)
            {
                var vM = this.DataContext as FamilyBrowserVM;
                //调用上传
                vM.FileUplod();
            }
            else
            {
                MessageBox.Show("未登录，请您先登录");
            }
        }


        //设置计时器多少秒实现一次
        const int Time = 300;
        //初始化
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var vM = this.DataContext as FamilyBrowserVM;
            vM.DeleteFile();
            vM.copy();
            //时间
            System.Timers.Timer t = new System.Timers.Timer(Time * 1000);
            t.Elapsed += new System.Timers.ElapsedEventHandler(Theout);
            //设置是执行一次（false）还是一直执行(true)；
            t.AutoReset = true;
            //启动计时器
            t.Enabled = true;
            UnfoldTreeview();
        }


        //计时器事件
        public void Theout(object source, System.Timers.ElapsedEventArgs e)
        {
            //if (IsLoaded)
            //{
            //    var vM = this.DataContext as FamilyBrowserVM;
            //    vM.IsDownload();
            //}
        }
        //展开树形节点
        private void UnfoldTreeview()
        {
            foreach (var item in Treeview1.Items)
            {
                DependencyObject dObject = Treeview1.ItemContainerGenerator.ContainerFromItem(item);
                ((TreeViewItem)dObject).IsExpanded = true;
            }
        }
        //通过审核
        private void audit_Click(object sender, RoutedEventArgs e)
        {
            var vM = this.DataContext as FamilyBrowserVM;
            CataItem item = Treeview1.SelectedItem as CataItem;
            if (item.newname.Length > 2)
            {
                //修改
                vM.PassAuditUpdate(item.Id, item.newname);
                //UnfoldTreeview();
            }
            else if (item.Audit == "未审核")
            {
                vM.PassAuditAdd(item.Id);
                //UnfoldTreeview();
            }
            vM.FileUplod();
        }
        //拒绝审核
        private void AuditRefuse_Click(object sender, RoutedEventArgs e)
        {
            CataItem item = Treeview1.SelectedItem as CataItem;
            var vM = this.DataContext as FamilyBrowserVM;
            if (item.newname.Length > 2)
            {
                vM.AuditRefuse(item.Id);
                //UnfoldTreeview();
            }
            else if (item.Audit == "未审核")
            {
                vM.AuditRefuseadd(item.Id);
                CataItem parent = item.Parent as CataItem;
                if (parent != null)
                {
                    //在父节点中删除选中的子节点                 
                    parent.Children.Remove(item);
                    item.Identifying = Convert.ToInt32(CataItem.Stater.Delete);
                    //UnfoldTreeview();
                }
            }
            vM.FileUplod();

        }

        //转换为实际应用中的大小（KB/M）
        public static string CountSize(long Size)
        {
            string m_strSize = "";
            long FactSize = 0;
            FactSize = Size;
            if (FactSize < 1024.00)
                m_strSize = FactSize.ToString("F2") + " Byte";
            else if (FactSize >= 1024.00 && FactSize < 1048576)
                m_strSize = (FactSize / 1024.00).ToString("F2") + " K";
            else if (FactSize >= 1048576 && FactSize < 1073741824)
                m_strSize = (FactSize / 1024.00 / 1024.00).ToString("F2") + " M";
            else if (FactSize >= 1073741824)
                m_strSize = (FactSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " G";
            return m_strSize;
        }
        //文件大小（字节）
        public static long GetFileSize(string sFullName)
        {
            long lSize = 0;
            if (File.Exists(sFullName))
            {
                lSize = new FileInfo(sFullName).Length;
            }
            return lSize;
        }
        //模型删除
        private void modeldelete_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoginState)
            {
                if (name != null)
                {
                    MessageBoxResult confirmToDel = MessageBox.Show("确认要删除" + name + "吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (confirmToDel == MessageBoxResult.Yes)
                    {
                        var vM = this.DataContext as FamilyBrowserVM;
                        if (vM.modeldelete(md5))
                        {
                            this.dataGrid.ItemsSource = vM.Modellist();
                        }
                    }
                }

            }
            else
            {
                MessageBox.Show("请先登录！");
            }

        }

        //模型下载
        private void modeldownload_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoginState)
            {
                if (name != null)
                {
                    MessageBoxResult confirmToDel = MessageBox.Show("确认要下载" + name + "文件吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (confirmToDel == MessageBoxResult.Yes)
                    {
                        var vM = this.DataContext as FamilyBrowserVM;
                        if (vM.ModelDownload(md5, this.filename.Text, name))
                        {
                            MessageBox.Show("下载成功");
                            this.filename.Text = "";
                            md5 = null;
                            name = null;
                        }
                        else
                        {
                            this.filename.Text = "下载失败";
                        }
                    }
                }
                else
                {
                    this.filename.Text = "请您先选择要下载的文件";
                }

            }
            else
            {
                this.filename.Text = "请您先登录";

            }
        }



        private void select_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = ""
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                //获取文件大小
                modelsize = CountSize(GetFileSize(openFileDialog.FileName)).ToString();
                //获取文件名字
                modelname = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                this.filename.Text = openFileDialog.FileName;
            }
        }
        //模型上传
        private void modelupload_Click_1(object sender, RoutedEventArgs e)
        {
            if (!isLoginState)
            {
                if (this.filename.Text != "")
                {
                    //if (classname != null)
                    //{
                    //    MessageBoxResult confirmToDel = MessageBox.Show("确认要上传" + modelname + "模型" + "到" + classname + "吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    //    if (confirmToDel == MessageBoxResult.Yes)
                    //    {
                    var vM = this.DataContext as FamilyBrowserVM;
                    string a = this.filename.Text;
                    if (vM.Modelupload(System.IO.Path.GetFullPath(this.filename.Text)))
                    {

                        this.dataGrid.ItemsSource = vM.Modellist();
                        this.filename.Text = "";
                        MessageBox.Show("上传成功！");
                    }
                    else
                    {
                        this.filename.Text = "上传失败。";
                    }
                    //}
                    //}
                    //else
                    //{
                    //   MessageBox.Show("请选择模型要添加到的类型");
                    //}
                }
                else
                {
                    MessageBox.Show("请选择需要上传的文件");
                }

            }
            else
            {
                MessageBox.Show("请您先登录");
            }
        }

        private void downloadpath_Click(object sender, RoutedEventArgs e)
        {
            string path = null;
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = fbd.SelectedPath;
                this.filename.Text = path;
            }

        }

        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (sender != null)
            {
                DataGrid grid = sender as DataGrid;
                if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                {
                    Model item = dataGrid.SelectedItem as Model;
                    md5 = item.MD5;
                    name = item.Mod_Name;
                }
            }
        }

        private void Treeview1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //CataItem item = Treeview1.SelectedItem as CataItem;
            //classname = item.Name;
            //catalogid = item.Id;
            //var vM = this.DataContext as FamilyBrowserVM;
            //this.dataGrid.ItemsSource = vM.list(item.Id);
          
        }
     

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                var vM = this.DataContext as FamilyBrowserVM;
                DataGrid grid = sender as DataGrid;
                if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                {
                    Model item = dataGrid.SelectedItem as Model;
                    vM.ismodelfile(item.MD5);               
                }
            }
        }

        private void renovation_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoginState)
            {
            FamilyBrowserVM vm = this.DataContext as FamilyBrowserVM;
            vm.Downloadnew();
            }
            else
            {
                MessageBox.Show("请您先登录！");
            }
           
        }
    }

}
