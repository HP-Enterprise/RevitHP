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

            var western = new Conference("电气专业")
            {
                Teams =
                {
                    new Team("市区供配电系统"),
                    new Team("应急柴油发电系统"),
                    new Team("VPS系统"),
                    new Team("接地系统"),
                    new Team("其他"),                   
                }
            };
            var eastern = new Conference("暖通专业")
            {

                Teams =
                {
                    new Team("冷水机组"),
                    new Team("冷水系统"),
                    new Team("机房末端精密空调及盘管"),
                    new Team("其他"),
                   
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

            DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("GroupCaalassification", typeof(string));
            dt.Columns.Add("Time", typeof(DateTime));
            dt.Columns.Add("ReviewTheStatus", typeof(string));
            dt.Columns.Add("FineDegreeOf", typeof(string));
            DataRow row = dt.NewRow();
            row["ID"] = 1;
            row["Name"] = "张三";
            row["GroupCaalassification"] = "123456";
            row["Time"] = "2017-7-7";
            row["FineDegreeOf"] = "数据1";
            row["ReviewTheStatus"] = "未审核";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["ID"] = 2;
            row["Name"] = "李四";
            row["GroupCaalassification"] = "789001";
            row["Time"] = "2017-1-1";
            row["FineDegreeOf"] = "数据2";
            row["ReviewTheStatus"] = "未审核";
            dt.Rows.Add(row);

            //dataGrid1.DataContext = dt;  

            Datagrid1.ItemsSource = dt.DefaultView;
           

        }

        private DataTable DataFamily()
        {
            DataTable dt = new DataTable();
            DataRow dr = dt.NewRow();
            dr["ID"] = 1;
            dr["Name"] ="家族1";
            dr["GroupCaalassification"] ="数据1";
            dr["FineDegreeOf"] = "数据1";
            dr["Time"] = "2017-1-1-";
            dr["ReviewTheStatus"] = "未审核";
            dt.Rows.Add(dr);        
            return dt;

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
            var item = (Team)Treeview1.SelectedItem;
            string tex = item.Name.ToString();
            MessageBox.Show(tex);
        }

        private void Datagrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //两种方式获取ID
            //第一种
            DataRowView mySelectedElement = (DataRowView)Datagrid1.SelectedItem;
            string result = mySelectedElement.Row[0].ToString();

            //第二种
            //var Txt = Datagrid1.SelectedItem as DataRowView;
            //string result = Txt.Row[0].ToString();
            MessageBox.Show(result);
        }
    }
}
