using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RevitHP
{


    class RevitBiz
    {
        // 缓存文件夹
        private string m_folder;

        // 数据库
        private LiteDB m_liteDB;

        // 层级结构
        Dictionary<int, CataItem> dictCatalog = null;

        public RevitBiz()
        {
            // 建立缓存文件夹
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            m_folder = Path.Combine(appData, "RevitHP");
            Directory.CreateDirectory(m_folder);

            m_liteDB = new LiteDB(m_folder);

        }

        // 初始化
        public void init()
        {
            // 1. 比较本地缓存和远程的MD5哈希值
            //    + 如果本地存在缓存，打开缓存数据库；如果本地不存在缓存，初始化一个空白的数据库Rev=0
            //    + 从远程查询查询数据库的最新MD5哈希值
            // 2. 比较结果有2种可能
            //    + 相等。表明本地的缓存已经是最新的
            //    + 不相等。多数情况表明远程服务器上有新版本，少数情况表明远程本地同时有修订，放弃本地缓存，使用远程版本
            // 3. 检查是否需要升级数据库的schema

            // TODO: 远程相关的逻辑还没有准备好，先使用本地的
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
                if (dictCatalog == null)
                {
                    LoadCatalog();
                }

                return dictCatalog[1];
            }
        }

        private void pull()
        {
            // TODO: 从远程拉一个数据库的新版本下来
        }

        private void push()
        {
            // TODO: 把本地数据库推送到远端
            // 为了安全，强制要求远程服务器执行检查逻辑
            // PUSH时带上修订前数据库的MD5值，如果和服务器端的不匹配，表明已经有其它修订发生
            // 服务器端直接返回失败,要求客户端重新PULL新的修订版后再重试PUSH
        }

        public ObservableCollection<CataItem> LoadCatalog()
        {

            var dictPID = new Dictionary<int, int>();
            dictCatalog = new Dictionary<int, CataItem>();
            using (var cmd = m_liteDB.CreateCommand())
            {

                cmd.CommandText = "SELECT id,name,parent FROM catalog";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        //组装字典
                        CataItem item = new CataItem();
                        item.Id = reader.GetInt32(0);
                        item.Name = reader.GetString(1);
                        dictCatalog.Add(reader.GetInt32(0), item);
                        //0位当前id, 2位父节点id
                        dictPID.Add(item.Id, reader.GetInt32(2));
                        Debug.WriteLine(dictCatalog.Keys.ToString());
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
                //获取 root 节点               
                //var root = dictCatalog.Values.First(c => c.Parent == null);
                ////获取二级节点
                ////建立 专业 节点(二级节点 )
                //var lst = dictCatalog.Values.Where(c => c.ParentID == root.ParentID);
                                  
                return new ObservableCollection<CataItem>(dictCatalog.Values);

            }
        }




    }
}
