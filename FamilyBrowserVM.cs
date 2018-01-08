using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitHP
{
    public class FamilyBrowserVM : INotifyPropertyChanged
    {
        // Revit业务层逻辑
        private RevitBiz m_biz = new RevitBiz();

        public event PropertyChangedEventHandler PropertyChanged;

        public FamilyBrowserVM()
        {
            m_biz.init();
        }

        //绑定树形结点
        public Collection<CataItem> TreeViewBinding
        {          
            get
            {
                Debug.WriteLine(m_biz.Top.Name);
                var league = new Collection<CataItem>() { m_biz.Top };
                return league;
            }
           
        }

        //登录
        public async Task<bool> isloginAsync(string Name, string Password)
        {
            bool islogin = await m_biz.IsloginAsync(Name, Password);        
            return islogin;
        }

        //注销
        public async Task<bool> islogout()
        {
            bool islogout = await m_biz.IslogoutAsync();
            return islogout;
        }

        public string md5()
        {
            return m_biz.md5();

        }
        public void OpenDB()
        {
            m_biz.openDB();
            //调用接口事件
            PropertyChanged(this, new PropertyChangedEventArgs("TreeViewBinding"));
        }

      
        public void IsDownload()
        {
            m_biz.pull();
        }

        public void copy()
        {
            m_biz.copy();
        }
        public void FileUplod()
        {
            m_biz.ispush();
        }

        public void DeleteFile()
        {
            m_biz.DeleteFile();
        }

    }
}
