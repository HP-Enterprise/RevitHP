using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            var western = new Conference("族库")
            {
                Teams =
                {
                    new Team("电气专业"){ Players = { "市电供", "三级菜单显示" }},
                    new Team("暖通专业"),
                    new Team("给排水专业及消防专业"){ Players = { "三级菜单显示", "三级菜单显示" }},
                    new Team("弱电专业"),
                    new Team("建筑结构及装修专业"),                   
                }
            };
            var eastern = new Conference("主目录")
            {

                Teams =
                {
                    new Team("测试数据二"),
                    new Team("测试数据二"),
                    new Team("测试数据二"),
                    new Team("测试数据二"),
                    new Team("测试数据二"),
                    new Team("测试数据二"),
                    new Team("测试数据二"),
                    new Team("Philadelphia Union 2010")
                }
            };
            var three = new Conference("主目录")
            {
                Teams =
                {
                    new Team("测试数据二"),
                    new Team("测试数据二"),
                    new Team("测试数据二"),
                    new Team("测试数据二"),
                    new Team("测试数据二"),
                    new Team("测试数据二"),
                    new Team("测试数据二"),
                    new Team("Philadelphia Union 2010")
                }
            };
            var league = new Collection<Conference>() { western, eastern, three };

            
            DataContext = new
            {
                WesternConference = western,
                EasternConference = eastern,
                League = league
            };

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


    }
}
