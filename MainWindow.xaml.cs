
﻿using System;
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


        public MainWindow()
        {
            InitializeComponent();

        }
        public static bool isLoginState = true;
        
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
                //在父节点中删除选中的子节点
                vm.SetCatalogdelete(item.Id);
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
                if (login.ShowDialog()==true)
                {
                    this.welcome.Content =ServerManagement.roleName;
                    this.login_state.Text = "注销";
                    vm.Downloadnew();
                    
                }
            }
            else
            {
                logoutAsync();
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
                this.welcome.Content = "未登录";
                this.login_state.Text = "登录";
            }
        }


        //上传事件      
        private void uploading_Click(object sender, RoutedEventArgs e)
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


        private void MD5_Click(object sender, RoutedEventArgs e)
        {

            var vM = this.DataContext as FamilyBrowserVM;
            vM.OpenDB();
       
            //UnfoldTreeview();

        
        }


        //设置计时器多少秒实现一次
        const int Time = 300;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var vM = this.DataContext as FamilyBrowserVM;
            vM.DeleteFile();
            vM.copy();
            //时间
            System.Timers.Timer t = new System.Timers.Timer(Time * 1000);
            t.Elapsed += new System.Timers.ElapsedEventHandler(theout);
            //设置是执行一次（false）还是一直执行(true)；
            t.AutoReset = true;
            //启动计时器
            t.Enabled = true;
            UnfoldTreeview();

            //if (ServerManagement.id!=1)
            //{
            // audit.Visibility = Visibility.Collapsed;
            //}


        }
        //计时器事件
        public void theout(object source, System.Timers.ElapsedEventArgs e)
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
                //((TreeViewItem)dObject).Background = Brushes.Aqua;
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
            }
            else if (item.Audit == 1)
            {
                vM.PassAuditAdd(item.Id);
            }
        }

        private void AuditRefuse_Click(object sender, RoutedEventArgs e)
        {
            CataItem item = Treeview1.SelectedItem as CataItem;
            var vM = this.DataContext as FamilyBrowserVM;
            if (item.newname.Length > 2)
            {
                vM.AuditRefuse(item.Id);
            }
            else if (item.Audit == 1)
            {
               
            }

        }

        private void ceshixiazai_Click(object sender, RoutedEventArgs e)
        {
            var vM = this.DataContext as FamilyBrowserVM;
            vM.Downloadnew();
        }
    }

}
