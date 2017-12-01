using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitHP
{
    class FamilyBrowserVM
    {
        // Revit业务层逻辑
        private RevitBiz m_biz = new RevitBiz();

        public FamilyBrowserVM()
        {
        }
        //绑定树形结点
        public Collection<CataItem> TreeViewBinding
        {
            
            get
            {
                
                m_biz.init();              
                var root = m_biz.LoadCatalog().First(c => c.Parent == null);
                var league = new Collection<CataItem>() { root };
                return league;
            }
            set
            {
            }
        }

    }
}
