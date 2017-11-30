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

        private string m_status = "I'm Ready!";
        public string status
        {
            get {
                m_biz.init();
                //var top = m_biz.Top;
                return m_status;
            }
            set {
                m_status = value;
            }
        }
        
        public Collection<CataItem> TreeViewBinding
        {
            get
            {
                m_biz.init();
                var items = m_biz.LoadCatalog();
                var root = items.First(c => c.Parent == null);
                var league = new Collection<CataItem>() { root }; 
                return league;
            }
            set
            {

            }
           

        }

    }
}
