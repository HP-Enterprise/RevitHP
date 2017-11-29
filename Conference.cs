using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitHP
{
   public class Conference
    {
        public Conference(string name)
        {

            Name = name;
            Items = new ObservableCollection<CataItem>();
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public ObservableCollection<CataItem> Items { get; set; }
       
    }
}
