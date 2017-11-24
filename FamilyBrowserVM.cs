using System;
using System.Collections.Generic;
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
                return m_status;
            }
            set {
                m_status = value;
            }
        }
    }
}
