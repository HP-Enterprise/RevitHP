using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;

namespace RevitHP
{
    // 小型数据库
    class LiteDB : IDisposable
    {
        // 文件名
        private const string c_liteName = "RevHP";

        // SQLite所在文件夹
        private string m_folder;

        // SQLite
        private SQLiteConnection m_cnn;
        //sqlliteDB 所在路径
        public static string sqlDBpath;


        public LiteDB(string folder)
        {
            m_folder = folder;
        }

        // 数据库修订号
        public uint Rev { get; private set; }

        // 在本机磁盘上初始化一个空白的数据库
        public void InitSQLite()
        {
            // 重置版本
            this.Rev = 0;

            // Create SQLite file
            SQLiteConnection.CreateFile(ResolveSQLiteFile());
        }

   
        public void Open(uint rev)
        {
            Debug.Assert(m_cnn == null);
            this.Rev = rev;
            var pathSQLiteFile = ResolveSQLiteFile();
            sqlDBpath = ResolveSQLiteFile();
            m_cnn = new SQLiteConnection($"Data Source={pathSQLiteFile};Version=3;");         
            m_cnn.Open();
        }
       
        public void Close()
        {
            if (m_cnn != null) {
                m_cnn.Close();
                m_cnn = null;
            }
        }
       


        // IDisposable
        public void Dispose()
        {
            Close();
        }

        public string ResolveSQLiteFile()
        {
            return Path.Combine(m_folder, $"{c_liteName}.{this.Rev}");
        }

        // 检查本地缓存的数据的版本
        public static uint? checkCachedRev(string folder)
        {
            uint? max = null;
            uint rev;
            foreach (var file in Directory.GetFiles(folder, $"{c_liteName}.*"))
            {
                string ext = Path.GetExtension(file);
                if (uint.TryParse(ext.Substring(1), out rev))
                {
                    if (max == null || max.Value < rev) {
                        max = rev;
                    }
                }
            }
            return max;
        }

        // 升级数据库
        public void Upgrade()
        {
            // 检查脚本及MD5值
            // 只需要检查最后一个脚本即可,先前的脚本我们视为已经发布,不可以再修订了
            string script = "20171124";

            var ri = LoadScript(script);

            using (ri.Stream)
            using (var cmd = m_cnn.CreateCommand())
            {
                string md5 = CalcMD5(ri.Stream);

                cmd.CommandText = $"SELECT md5 FROM version WHERE script = @script";
                cmd.Parameters.AddWithValue("@script", script);

                bool bMatched;
                try
                {
                    string scriptMD5 = cmd.ExecuteScalar() as string;
                    bMatched = (scriptMD5 == md5);
                }
                catch (SQLiteException) {
                    // 表不存在
                    bMatched = false;
                }

                if (!bMatched) {
                    // rollback
                    var rb = LoadScript($"{script}~");
                    Exec(rb.Stream, cmd);

                    // patch
                    ri.Stream.Seek(0, SeekOrigin.Begin);
                    Exec(ri.Stream, cmd);

                    cmd.CommandText = "REPLACE INTO version(script,md5,tm) VALUES(@script,@md5,datetime('now'))";
                    cmd.Parameters.AddWithValue("@md5", md5);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public SQLiteDataReader ExecuteReader(SQLiteCommand cmd)
        {
            return cmd.ExecuteReader();
        }

        public object ExecuteScalar(SQLiteCommand cmd)
        {
            return cmd.ExecuteScalar();
        }

        public int ExecuteNoQuery(SQLiteCommand cmd)
        {
            return cmd.ExecuteNonQuery();
        }

        public SQLiteCommand CreateCommand()
        {
            return m_cnn.CreateCommand();
        }

        private void Exec(Stream stream, SQLiteCommand cmd = null)
        {
            SQLiteCommand myCmd = null;
            if (cmd == null) {
                myCmd = m_cnn.CreateCommand();
                cmd = myCmd;
            }

            try
            {
                using (var reader = new StreamReader(stream))
                {
                    cmd.CommandText = reader.ReadToEnd();
                    cmd.ExecuteNonQuery();
                }
            }
            finally {
                if (myCmd != null) myCmd.Dispose();
            }
        }

        // 获取资源信息
        private static StreamResourceInfo LoadScript(string script)
        {
            var asm = typeof(LiteDB).Assembly;
            var res = Application.GetResourceStream(new Uri($"pack://application:,,,/{asm.FullName};component/script/{script}.sql", UriKind.RelativeOrAbsolute));
            return res;
        }

        // 计算一个stream的md5值，以16进制大写字符串的形式返回结果
        private static string CalcMD5(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                var bufMD5 = md5.ComputeHash(stream);
                string strMD5 = BitConverter.ToString(bufMD5);
                return strMD5.Replace("-", String.Empty);
            }
        }


    }
}
