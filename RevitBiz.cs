using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RevitHP
{


    public class RevitBiz
    {

        // 缓存文件夹
        private string m_folder;

        // 数据库
        private LiteDB m_liteDB;


        //层级结构
        Dictionary<int, CataItem> dictCatalog = null;

        ModelManagement model = new ModelManagement();
        //实例
        ServerManagement server = new ServerManagement();
        // 单实例
        GetMD5HashFromFile getMD5 = new GetMD5HashFromFile();
        private static RevitBiz s_biz = new RevitBiz();


        public static RevitBiz Instance
        {
            get { return s_biz; }
        }
        protected RevitBiz()
        {
            // 建立缓存文件夹
            //在当前电脑用户下建立缓存文件夹
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //在项目运行环境下建立缓存文件夹
            //var appData = Environment.CurrentDirectory;
            //缓存文件路径
            m_folder = Path.Combine(appData, "RevitHP");
            Directory.CreateDirectory(m_folder);
            m_liteDB = new LiteDB(m_folder);
        }

        public void coloes()
        {
            m_liteDB.Close();
        }

        // 初始化
        public void init()
        {
            uint? rev = LiteDB.checkCachedRev(m_folder);
            if (rev == null)
            {
                m_liteDB.InitSQLite();
                rev = m_liteDB.Rev;
            }
            m_liteDB.Open(rev.Value);
            m_liteDB.Upgrade();

        }

        public CataItem Top
        {
            get
            {
                LoadCatalog();
                return dictCatalog[1];
            }
        }




        public int Pull()
        {
            m_liteDB.Close();
            int statuscode = server.DownloadStatusCode(m_folder + "\\RevHP.0", md5());
            switch (statuscode)
            {
                //200代表服务器有新的数据库
                case 200:
                    //下载最新的数据文件     
                    m_liteDB.Close();
                    if (server.DownloadNew(m_folder + "\\RevHP.0"))
                    {
                        copy();
                    }
                    return 200;
                //break;
                //304代表版本一致
                case 304:
                    OpenDB();
                    return 304;
                //404代表服务器是全新的
                case 404:
                    //服务器没有数据库，本地初始化一个            
                    init();
                    m_liteDB.Close();
                    copy2();
                    //第一次上传
                    if (server.FistPush(m_folder + "\\RevHP.2"))
                    {
                        File.Delete(m_folder + "\\RevHP.2");
                        m_liteDB.Open(1);
                    }
                    return 404;
                default:
                    return statuscode;
            }

        }


        //判断上传
        public void ispush()
        {
            //上传时，将文件.1复制.2,并将.2上传.
            copy2();
            if (server.Push(m_folder + "\\RevHP.2", m_folder + "\\RevHP.0"))
            {
                //如果上传成功
                //删除文件.0和文件.1
                //修改文件后缀，将.2修改为.0             
                File.Delete(m_folder + "\\RevHP.0");
                File.Delete(m_folder + "\\RevHP.1");
                string fileName = m_folder + "\\RevHP.2";
                string dfileName = Path.ChangeExtension(fileName, ".0");
                File.Move(fileName, dfileName);
                copy();
                //m_liteDB.Open(1);
            }
            else
            {
                //如果上传失败执行下载
                server.DownloadNew(m_folder + "\\RevHP.0");
                File.Delete(m_folder + "\\RevHP.2");
                //执行合并方法
            }
        }




        public void IsDownloadNew()
        {
            server.DownloadNew(m_folder + "\\RevHP.0");
            copy();
        }


        //读数据库树形节点
        private void LoadCatalog()
        {
            var dictPID = new Dictionary<int, int>();
            dictCatalog = new Dictionary<int, CataItem>();
            using (var cmd = m_liteDB.CreateCommand())
            {
                if (ServerManagement.id == 1)
                {
                    cmd.CommandText = "SELECT id,name,parent,newname,audit FROM catalog ";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //组装字典
                            CataItem item = new CataItem();
                            item.Id = reader.GetInt32(0);
                            item.Name = reader.GetString(1);
                            item.NewName = reader.GetString(3);
                            item.Audit = reader.GetInt32(4);
                            dictCatalog.Add(reader.GetInt32(0), item);
                            //0位当前id, 2位父节点id
                            dictPID.Add(item.Id, reader.GetInt32(2));
                            // Debug.WriteLine(dictCatalog.Keys.ToString());
                        }
                    }
                }
                else
                {
                    cmd.CommandText = string.Format("SELECT id,name,parent,newname FROM catalog where NameID={0} or audit=0 ", ServerManagement.id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CataItem item = new CataItem();
                            item.Id = reader.GetInt32(0);
                            item.Name = reader.GetString(1);
                            //item.NewName = reader.GetString(3);
                            //item.Audit = reader.GetInt32(4);
                            dictCatalog.Add(reader.GetInt32(0), item);
                            if (reader.GetString(3) != "")
                            {
                                item.Name = reader.GetString(3);
                            }
                            //组装字典                          
                            //0位当前id, 2位父节点id
                            dictPID.Add(item.Id, reader.GetInt32(2));
                            Debug.WriteLine(dictCatalog.Keys.ToString());
                        }
                    }
                }

                //对 父子节点进行组装
                foreach (var key in dictCatalog.Keys)
                {
                    //找到当前节点
                    var c = dictCatalog[key];
                    //获取当前节点的父节点id
                    var parentID = dictPID[c.Id];
                    if (dictCatalog.ContainsKey(parentID))
                    {
                        //获取父节点对象
                        var parent = dictCatalog[parentID];
                        //指定当前节点的父节点
                        c.Parent = parent;
                        //给父节点的Children集合加入当前节点
                        parent.Children.Add(c);
                    }
                }
            }
        }


        //写入数据库
        //添加
        public void SetCatalogAdd(string name, int id, int parentid)
        {
            using (var cmd = m_liteDB.CreateCommand())
            {
                cmd.CommandText = string.Format("insert into catalog(id,name,parent,identifying,audit,NameID) values('{0}','{1}','{2}','{3}','{4}','{5}')", id, name, parentid, 0, 1, ServerManagement.id);
                cmd.ExecuteScalar();
            }
        }

        //修改
        public void SetCatalogUpdate(int id, string newname)
        {
            using (var cmd = m_liteDB.CreateCommand())
            {
                cmd.CommandText = string.Format("UPDATE catalog set newname='{0}',identifying='{1}',audit='{2}',NameID='{4}' where id={3}", newname, 2, 0, id, ServerManagement.id);
                cmd.ExecuteScalar();
            }

        }
        //删除
        public void SetCatalogDelete(int id)
        {
            using (var cmd = m_liteDB.CreateCommand())
            {
                cmd.CommandText = string.Format("Delete from catalog where id='{0}'", id);
                cmd.ExecuteScalar();
            }

        }

        //登录
        public bool Login(string name, SecureString pwd)
        {
            return server.HttpClientDoPostLogin(name, pwd);

        }
        //登录
        public bool IsloginAsync(string Name, SecureString Password)
        {
            return server.HttpClientDoPostLogin(Name, Password);
        }
        //注销 
        public async Task<bool> IslogoutAsync()
        {
            return await server.HttpClientDoPostLogout();
        }
        //判断MD5码
        public string md5()
        {
            //关闭sqlliteDB
            //m_liteDB.Close();
            if (File.Exists(LiteDB.sqlDBpath))
            {
                string filepath = m_folder + "\\RevHP.0";
                string md5 = getMD5.GetMD5Hash(filepath);
                return md5;
            }
            else
            {
                //如果文件不存在,返回指定MD5码，下载最新的版本文件               
                //返回一个故意错误的MD5
                return "2222222222222222222222222222222";
            }
        }
        //打开数据库
        public void OpenDB()
        {
            m_liteDB.Open(1);
            m_liteDB.Upgrade();
        }

        //合并（测试）
        public void Updatetreeview()
        {
            List<int> parentid = new List<int>();
            List<CataItem> list = new List<CataItem>();
            using (var cmd = m_liteDB.CreateCommand())
            {
                //查询所有修改过后的结点
                cmd.CommandText = "SELECT id,newname,parent FROM catalog where identifying =2";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //组装字典                     
                        CataItem item = new CataItem();
                        int a = reader.GetInt32(0);
                        string ab = reader.GetString(1);
                        item.Id = reader.GetInt32(0);
                        item.Name = reader.GetString(1);
                        parentid.Add(reader.GetInt32(2));
                        list.Add(item);
                    }
                }
            }

            LiteDB NewliteDB = new LiteDB(m_folder);
            NewliteDB.Open(0);

            using (var newcmd = NewliteDB.CreateCommand())
            {
                for (int j = 0; j < list.Count; j++)
                {
                    newcmd.CommandText = string.Format("SELECT id,name,parent FROM catalog where id='{0}'", parentid[j]);
                    using (var updatereader = newcmd.ExecuteReader())
                    {
                        if (updatereader.Read())
                        {
                            using (var updatecom = NewliteDB.CreateCommand())
                            {
                                updatecom.CommandText = string.Format("UPDATE catalog set name='{0}',audit='{1}' where id={2}", list[j].Name, 1, list[j].Id);
                                updatecom.ExecuteScalar();
                            }
                        }
                        else
                        {
                            using (var cmd = m_liteDB.CreateCommand())
                            {

                                cmd.CommandText = string.Format("SELECT id,name,parent FROM catalog where id='{0}'", parentid[j]);
                                using (var readerparent = cmd.ExecuteReader())
                                {
                                    while (readerparent.Read())
                                    {
                                        using (var addcom = NewliteDB.CreateCommand())
                                        {
                                            addcom.CommandText = string.Format("insert into catalog(id,name,parent) values('{0}','{1}','{2}')", readerparent.GetInt32(0), readerparent.GetString(1), readerparent.GetInt32(2));
                                            addcom.ExecuteScalar();
                                        }
                                    }
                                }

                                using (var updatecom = NewliteDB.CreateCommand())
                                {
                                    updatecom.CommandText = string.Format("UPDATE catalog set name='{0}',audit='{1}' where id={2}", list[j].Name, 1, list[j].Id);
                                    updatecom.ExecuteScalar();
                                }
                            }

                        }
                    }
                }
            }
            NewliteDB.Close();
        }
        //合并（测试）
        public string OpenDB1()
        {
            List<CataItem> list = new List<CataItem>();
            List<int> parentid = new List<int>();
            using (var cmd = m_liteDB.CreateCommand())
            {
                cmd.CommandText = "SELECT id,name,parent FROM catalog where identifying =0";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //组装字典                     
                        CataItem item = new CataItem();
                        item.Id = reader.GetInt32(0);
                        item.Name = reader.GetString(1);
                        parentid.Add(reader.GetInt32(2));
                        list.Add(item);
                    }
                }
            }

            //打开下载的文件
            LiteDB NewliteDB = new LiteDB(m_folder);
            NewliteDB.Open(0);
            using (var newcmd = NewliteDB.CreateCommand())
            {

                for (int j = 0; j < parentid.Count; j++)
                {
                    newcmd.CommandText = string.Format("SELECT id,name,parent FROM catalog where id='{0}'", parentid[j]);
                    using (var reader = newcmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            using (var addcom = NewliteDB.CreateCommand())
                            {
                                addcom.CommandText = string.Format("insert into catalog(id,name,parent,audit) values('{0}','{1}','{2}','{3}')", list[j].Id, list[j].Name, parentid[j], 1);
                                addcom.ExecuteScalar();
                            }
                        }
                        else
                        {
                            using (var cmd = m_liteDB.CreateCommand())
                            {
                                cmd.CommandText = string.Format("SELECT id,name,parent FROM catalog where id='{0}'", parentid[j]);
                                using (var readerparent = cmd.ExecuteReader())
                                {
                                    while (readerparent.Read())
                                    {
                                        using (var addcom = NewliteDB.CreateCommand())
                                        {
                                            addcom.CommandText = string.Format("insert into catalog(id,name,parent) values('{0}','{1}','{2}')", readerparent.GetInt32(0), readerparent.GetString(1), readerparent.GetInt32(2));
                                            addcom.ExecuteScalar();
                                        }
                                    }
                                }

                                using (var addcom = NewliteDB.CreateCommand())
                                {
                                    addcom.CommandText = string.Format("insert into catalog(id,name,parent,audit) values('{0}','{1}','{2}','{3}')", list[j].Id, list[j].Name, parentid[j], 1);
                                    addcom.ExecuteScalar();
                                }
                            }

                        }
                    }
                }

            }
            NewliteDB.Close();
            Updatetreeview();
            return "a";
        }



        //将文件.0复制为.1并打开它
        public void copy()
        {
            m_liteDB.Close();
            string sourceFile = m_folder + "\\RevHP.0";
            string destinationFile = m_folder + "\\RevHP.1";
            FileInfo file = new FileInfo(sourceFile);
            if (file.Exists)
            {
                file.CopyTo(destinationFile, true);
            }
            OpenDB();
        }
        //将文件.1复制为.2，并去掉所有标识。
        public void copy2()
        {
            m_liteDB.Close();
            string sourceFile = m_folder + "\\RevHP.1";
            string destinationFile = m_folder + "\\RevHP.2";
            FileInfo file = new FileInfo(sourceFile);
            if (file.Exists)
            {
                file.CopyTo(destinationFile, true);
                //打开数据库 
                m_liteDB.Open(2);
                using (var cmd = m_liteDB.CreateCommand())
                {
                    cmd.CommandText = string.Format("UPDATE catalog set identifying=null");
                    cmd.ExecuteScalar();
                }
                m_liteDB.Close();
            }
        }

        //初始化时候，判断是否有.1和.2文件，如果有，删除它们
        public void DeleteFile()
        {
            m_liteDB.Close();
            if (File.Exists(m_folder + "\\RevHP.0"))
            {
                File.Delete(m_folder + "\\RevHP.0");//删除该文件
            }
            if (File.Exists(m_folder + "\\RevHP.2"))
            {
                File.Delete(m_folder + "\\RevHP.2");//删除该文件
            }
            m_liteDB.Open(1);
        }
        //通过审核（添加）
        public void PassAuditAdd(int id)
        {
            using (var cmd = m_liteDB.CreateCommand())
            {
                cmd.CommandText = string.Format("UPDATE catalog set audit='{0}' where id={1}", 0, id);
                cmd.ExecuteScalar();
            }
        }
        //通过审核（修改）
        public void PassAuditUpdate(int id, string newname)
        {
            using (var cmd = m_liteDB.CreateCommand())
            {
                cmd.CommandText = string.Format("UPDATE catalog set audit='{0}',name='{2}',nameID=null,newname='' where id={1}", 0, id, newname);
                cmd.ExecuteScalar();
            }
        }
        //审核拒绝（修改）
        public void AuditRefuse(int id)
        {
            using (var cmd = m_liteDB.CreateCommand())
            {
                cmd.CommandText = string.Format("UPDATE catalog set audit=0 ,newname='' where id='{0}'", id);
                cmd.ExecuteScalar();
            }

        }
        //审核拒绝（添加）
        public void AuditRefuseadd(int id)
        {
            using (var cmd = m_liteDB.CreateCommand())
            {
                cmd.CommandText = string.Format("Delete from catalog where id='{0}'", id);
                cmd.ExecuteScalar();
            }

        }

        //模型上传
        public bool Modelupload(string filepath)
        {
           return  model.Modelupload(filepath);
        }
        //模型删除
        public bool modeldelete(string md5)
        {
          return  model.ModelDelete(md5);
        }
        //模型下载
        public bool ModelDownload(string md5)
        {
           return  model.ModelDownload(m_folder+"/"+md5+".rfa", md5);
        }
        //模型列表
        public List<Model> ModelList()
        {
            return model.ModelFileList();
        }

        //测试
        public List<Model> GetList(int id)
        {        
            List<Model> list = new List<Model>();
            using (var cmd = m_liteDB.CreateCommand())
            {
                cmd.CommandText =string.Format("SELECT id,mod_name,mod_size,catalogid FROM Model where catalogid={0}",id);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //组装字典
                        Model mod = new Model();
                        mod.Id = reader.GetInt32(0);
                        mod.Mod_Name = reader.GetString(1);
                        mod.Mod_Size = reader.GetString(2);
                        mod.CatalogId = reader.GetInt32(3);
                        list.Add(mod);
                    }
                }
            }
            return list;
        }
    }
}

