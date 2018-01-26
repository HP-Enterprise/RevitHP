using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace RevitHP
{
    /// <summary>
    /// TreeIndex.xaml 的交互逻辑
    /// </summary>
    public partial class TreeIndex : Window
    {

        private CataItem _parentItem;
        private FamilyBrowserVM Vm;
        private static Random ra = new Random();
        public TreeIndex(CataItem item, FamilyBrowserVM familyBrowserVM)
        {
            InitializeComponent();
            _parentItem = item;
            this.ParentName.Content = item.Name;
            Vm = familyBrowserVM;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.NodeInput.Text != "")
            {
                CataItem list = new CataItem();
                if (ServerManagement.id == 1)
                {
                    Vm.SetCatalogAdminAdd(this.NodeInput.Text, ra.Next(), _parentItem.Id);
                }
                else
                {
                    Vm.SetCatalogAdd(this.NodeInput.Text, ra.Next(), _parentItem.Id);
                }

                list.Identifying = Convert.ToInt32(CataItem.Stater.Input);
                list.Name = this.NodeInput.Text;
                _parentItem.Children.Add(list);
                this.Close();
            }
            else
            {
                this.pointout.Text = "添加节点名称不能为空";
            }


        }
    }
}
