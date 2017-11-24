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
        public Team(string name)
        {
            Name = name;
            Players = new ObservableCollection<string>();
        }

        public string Name { get; private set; }
        public ObservableCollection<string> Players { get; private set; }
        ObservableCollection<string> Paaa = new ObservableCollection<string>();
        
    }
}
