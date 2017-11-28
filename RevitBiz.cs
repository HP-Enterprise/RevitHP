using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void init() {
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
                if (dictCatalog == null) {
                    LoadCatalog();
                }

                return null;
                // return dictCatalog[1];
            }
        }

        private void pull() {
            // TODO: 从远程拉一个数据库的新版本下来
        }

        private void push() {
            // TODO: 把本地数据库推送到远端
            // 为了安全，强制要求远程服务器执行检查逻辑
            // PUSH时带上修订前数据库的MD5值，如果和服务器端的不匹配，表明已经有其它修订发生
            // 服务器端直接返回失败,要求客户端重新PULL新的修订版后再重试PUSH
        }

        private void LoadCatalog()
        {
            using (var cmd = m_liteDB.CreateCommand())
            {
                cmd.CommandText = "SELECT id,name,parent FROM catalog";
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        // TODO: 构造 dictCatalog
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        int parent = reader.GetInt32(2);
                        Debug.WriteLine($"{id} {name} {parent}");
                    }
                }
            }
        }
    }
}
