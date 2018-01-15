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
        public bool isloginAsync(string Name, string Password)
        {
            bool islogin = m_biz.IsloginAsync(Name, Password);
            return islogin;
        }

        //注销
        public async Task<bool> islogout()
        {
            bool islogout = await m_biz.IslogoutAsync();
            return islogout;
        }

        public string OpenDB()
        {

            //RevitBiz biz = new RevitBiz();
            return m_biz.openDB1();

            //调用接口事件
            //PropertyChanged(this, new PropertyChangedEventArgs("TreeViewBinding"));
        }

        //文件下载
        public void IsDownload()
        {
            switch (m_biz.pull())
            {
                case 200:
                    PropertyChanged(this, new PropertyChangedEventArgs("TreeViewBinding"));
                    break;
                default:
                    break;
            }

        }

        public void copy()
        {
            m_biz.copy();
        }
        //文件上传
        public void FileUplod()
        {
            m_biz.ispush();
            PropertyChanged(this, new PropertyChangedEventArgs("TreeViewBinding"));
        }
        //删除文件
        public void DeleteFile()
        {
            m_biz.DeleteFile();
        }
        //添加结点
        public void SetCatalogAdd(string name, int id, int parentid)
        {
            m_biz.SetCatalogAdd(name, id, parentid);
        }
        //修改结点
        public void SetCatalogUpdate(int id, string newname)
        {
            m_biz.SetCatalogUpdate(id, newname);
        }
        //删除节点
        public void SetCatalogdelete(int id)
        {
            m_biz.SetCatalogDelete(id);
        }
        //文件测试
        public string ceshi()
        {
            return m_biz.selete();
        }
        //审核
        public void PassAuditAdd(int id)
        {
            m_biz.PassAuditAdd(id);
        }
        public void PassAuditUpdate(int id,string newname)
        {
            m_biz.PassAuditUpdate(id, newname);

        }
    }
}
