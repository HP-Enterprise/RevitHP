using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
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
    }
}
