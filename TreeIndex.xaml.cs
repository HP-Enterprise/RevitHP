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
        public TreeIndex(RevitHP.CataItem item)
        {
            
            InitializeComponent();
            _parentItem = item;          
            this.ParentName.Content = item.Name;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CataItem list = new CataItem();
            //list.Id = (_parentItem.Id * 10) + _parentItem.Children.Count + 1;
            //list.ParentID = 0;
            list.Identifying = Convert.ToInt32(CataItem.Stater.Input);
            list.Name = this.NodeInput.Text;
            _parentItem.Children.Add(list);        
            

        }
    }
}
