using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace RevitHP
{
    public class FamilyBrowserVM : INotifyPropertyChanged
    {
        // Revit业务层逻辑
        private RevitBiz m_biz =RevitBiz.Instance;

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
                // Debug.WriteLine(m_biz.Top.Name);
                var league = new Collection<CataItem>() { m_biz.Top };
                return league;
            }

        }

        //登录
        public bool isloginAsync(string Name, SecureString Password)
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

        public void renovation()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("TreeViewBinding"));
        }

        //测试合并
        public string OpenDB()
        {

            //RevitBiz biz = new RevitBiz();
            return m_biz.OpenDB1();

            //调用接口事件
            //PropertyChanged(this, new PropertyChangedEventArgs("TreeViewBinding"));
        }

        //文件下载
        public void IsDownload()
        {
            switch (m_biz.Pull())
            {
                case 200:
                    PropertyChanged(this, new PropertyChangedEventArgs("TreeViewBinding"));
                    break;
                default:
                    break;
            }

        }

        public void Downloadnew()
        {
            m_biz.IsDownloadNew();
            PropertyChanged(this, new PropertyChangedEventArgs("TreeViewBinding"));
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
        public void DeleteFile2()
        {
            m_biz.DeleteFile2();
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
       
        //审核
        public void PassAuditAdd(int id)
        {
            m_biz.PassAuditAdd(id);
        }
        public void PassAuditUpdate(int id,string newname)
        {
            m_biz.PassAuditUpdate(id, newname);

        }
        //审核失败
        public void AuditRefuse(int id)
        {
            m_biz.AuditRefuse(id);
        }
        public void AuditRefuseadd(int id)
        {
            m_biz.AuditRefuseadd(id);
        }


        //模型上传
        public bool Modelupload(string filepath)
        {
           return m_biz.Modelupload(filepath);
        }
        public bool modeldelete(string md5)
        {
           return m_biz.modeldelete(md5);
        }
        public bool ModelDownload(string md5,string path,string name)
        {
          return  m_biz.ModelDownload(md5,path,name);
        }
        public List<Model> Modellist()
        {
            return m_biz.ModelList();
        }
        public List<Model> list(int id)
        {
            return m_biz.GetList(id);
        }

    }
}
