using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitHP
{
   public class Team
    {
        //public Team(string name)
        //{
        //    Name = name;
        //    ChildrenForDisplay = new ObservableCollection<string>();
        //}

        //public string Name { get; set; }
        //public ObservableCollection<string> ChildrenForDisplay { get; set; }


        public Team(string name)
        {
            Name = name;
            ChildrenForDisplay = new ObservableCollection<CataItem>();
        }

        public string Name { get; private set; }
        public ObservableCollection<CataItem> ChildrenForDisplay { get; private set; }
        


    }
}
