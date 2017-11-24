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
            Teams = new ObservableCollection<Team>();
        }

        public string Name { get; set; }
        public ObservableCollection<Team> Teams { get; set; }
       
    }
}
