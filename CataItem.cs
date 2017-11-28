using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitHP
{
    class CataItem
    {
        public CataItem()
        {
            Children = new List<CataItem>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public CataItem Parent { get; set; }
        public List<CataItem> Children { get; private set; }
    }
}
