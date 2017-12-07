using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitHP
{
    public class FamilyBrowserVM
    {
        // Revit业务层逻辑
        private RevitBiz m_biz = new RevitBiz();

        public FamilyBrowserVM()
        {
            m_biz.init();
        }

        //绑定树形结点
        public Collection<CataItem> TreeViewBinding
        {          
            get
            {
                var league = new Collection<CataItem>() { m_biz.Top };
                return league;
            }
        }

        //绑定族基本信息
        //public Collection<FamilyMessage> familyMessagebinding
        //{
        //    get
        //    {
        //        var league = new Collection<FamilyMessage>() { };
        //        return league;
        //    }
        //}
    }
}
