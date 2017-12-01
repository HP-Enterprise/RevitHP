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

            /* REQUEST 请求
             * HTTP GET /{prefix}/catalog
             * HTTP HEAD
             *     If-None-Match: 可选.内容的MD5值(32个字符的大写16进制字符串形式，如 52291B5CECA35EEFABB4532CC462E1EC)
             * 
             * RESPONSE 响应
             * HTTP HEAD
             *     ETag: 内容的MD5值(32个字符的大写16进制字符串形式)
             *     Content-Type: application/octet-stream 字节流
             * HTTP Status Code
             *     200: 请求的If-None-Match的值不匹配(或没有发送If-None-Match)时,返回内容的二进制字节流
             *     304: 请求的If-None-Match值匹配时,不需要返回内容
             *     404: 如果客户端从来没有上传过数据，返回404错误
             * HTTP Content 可选.内容的二进制字节流
             */
        }

        private void push()
        {
            // TODO: 把本地数据库推送到远端
            // 为了安全，强制要求远程服务器执行检查逻辑
            // PUSH时带上修订前数据库的MD5值，如果和服务器端的不匹配，表明已经有其它修订发生
            // 服务器端直接返回失败,要求客户端重新PULL新的修订版后再重试PUSH

            /* REQUEST 请求
             * HTTP POST /{prefix}/catalog
             * HTTP HEAD
             *     If-Match: 期望服务器上的旧数据匹配此值(MD5的大写16进制字符串形式32个字符)
             *     Content-Type: application/octet-stream 字节流
             * HTTP Content 内容的二进制字节流
             * 
             * RESPONSE 响应
             * HTTP Status Code
             *     204: 请求中If-Match的内容和服务器上旧数据的值匹配，服务器用请求里的内容更新服务器上的数据
             *     412: 请求中If-Match的内容和服务器上旧数据的值不匹配，服务器拒绝请求，不更新数据，且服务器也不需要读取请求的内容
             */
        }

        private void LoadCatalog()
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
            }
        }




    }
}
