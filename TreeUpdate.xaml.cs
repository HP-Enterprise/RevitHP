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
    /// TreeUpdate.xaml 的交互逻辑
    /// </summary>
    public partial class TreeUpdate : Window
    {
        private CataItem _parentItem;
        public TreeUpdate(RevitHP.CataItem item)
        {
            InitializeComponent();
            _parentItem = item;
            this.ParentName.Content = item.Name.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.UpdateName.Text=="")
            {
                MessageBox.Show("修改名不能为空");
            }
            else
            {
                _parentItem.name = this.UpdateName.Text;
                _parentItem.Identifying = Convert.ToInt32(CataItem.Stater.Update);


                if (_parentItem.name== this.UpdateName.Text)
                {
                    this.Close();
                }
            }
          
        }
    }
}
