using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitHP
{
    public class CataItem
    {
        public CataItem(string name, int id)
        {
            Name = name;
            Id = id;
            Children = new List<CataItem>();
        }
        public CataItem()
        {        
            Children = new List<CataItem>();
        }
        public int ParentID
        {
            get
            {
                if (Parent != null)
                    return Parent.Id;
                else
                    return 0;
            }
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public CataItem Parent { get; set; }     
        public List<CataItem> Children { get; set; }
        public ObservableCollection<CataItem> Items
        {
            get 
            {
                return new ObservableCollection<CataItem>(Children);
            }
            
        }
        public CataItem(string name)
        {
            Name = name;            
        }
    }
}
