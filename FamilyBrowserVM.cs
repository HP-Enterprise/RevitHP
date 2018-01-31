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


        private string m_strErrorMsg = String.Empty;
        public string ErrorMsg
        {
            get { return m_strErrorMsg; }
            set
            {
                m_strErrorMsg = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ErrorMsg"));
            }
        }

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
        //下载最新版文件
        public void Downloadnew()
        {
            m_biz.IsDownloadNew();
            PropertyChanged(this, new PropertyChangedEventArgs("TreeViewBinding"));
        }
        //复制文件
        public void copy()
        {
            m_biz.copy();
        }
        //文件上传
        public bool FileUplod()
        {
           PropertyChanged(this, new PropertyChangedEventArgs("TreeViewBinding"));
           return  m_biz.ispush(); 
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
        //管理员添加结点
        public void SetCatalogAdminAdd(string name, int id, int parentid)
        {
            m_biz.SetCatalogAdminAdd(name, id, parentid);
        }
        //修改结点
        public void SetCatalogUpdate(int id, string newname)
        {
            m_biz.SetCatalogUpdate(id, newname);
        }
        //修改结点
        public void SetCatalogAdminUpdate(int id, string newname)
        {
            m_biz.SetCatalogAdminUpdate(id, newname);
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
            PropertyChanged(this, new PropertyChangedEventArgs("TreeViewBinding"));
        }
        public void PassAuditUpdate(int id,string newname)
        {
            m_biz.PassAuditUpdate(id, newname);
            PropertyChanged(this, new PropertyChangedEventArgs("TreeViewBinding"));

        }
        //审核失败
        public void AuditRefuse(int id)
        {
            m_biz.AuditRefuse(id);
            PropertyChanged(this, new PropertyChangedEventArgs("TreeViewBinding"));
        }
        public void AuditRefuseadd(int id)
        {
            m_biz.AuditRefuseadd(id);
            PropertyChanged(this, new PropertyChangedEventArgs("TreeViewBinding"));
        }


        //模型上传
        public bool Modelupload(string filepath)
        {
           return m_biz.Modelupload(filepath);
        }
        //模型删除
        public bool modeldelete(string md5)
        {
           return m_biz.modeldelete(md5);
        }
        //删除
        public bool modellistdelete(string md5)
        {
            return m_biz.isdeletelist(md5);
        }

        //模型下载
        public bool ModelDownload(string md5,string path,string name)
        {
          return  m_biz.ModelDownload(md5,path,name);
        }
        //模型列表查看
        public List<Model> Modellist()
        {
            return m_biz.ModelList();
        }
        public List<Model> list(int id)
        {
            return m_biz.GetList(id);
        }

        public List<Model> list()
        {
            return m_biz.GetList();
        }

        //模型判断  
        public void ismodelfile(string md5,string name)
        {
            m_biz.ismodelfile(md5,name);
        }
        //模型上传
        public bool isaddlist(int id, string name, string size, int catalogid,string md5)
        {
           return m_biz.isaddlist(id,name,size,catalogid,md5);
        }
        //模型审核成功
        public bool ispassmodel(string md5)
        {
            return m_biz.passmodel(md5);
        }
        public bool isrefusemodel(string md5)
        {
            return m_biz.isrefusemodel(md5);
        }

    }
}
