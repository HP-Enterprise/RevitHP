using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        private string ResolveSQLiteFile()
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
            int count = 0;
            using (var cmd = new SQLiteCommand(m_cnn)) {
                cmd.CommandText = "SELECT count(*) FROM sqlite_master WHERE type='table' AND name='version'";
                var obj = cmd.ExecuteScalar();
                count = Convert.ToInt32(obj);
            }

            if (count == 0) Exec("20171124.sql");
        }

        // 执行内崁脚本
        private void Exec(string script)
        {
            var asm = typeof(LiteDB).Assembly;
            var res = Application.GetResourceStream(new Uri($"pack://application:,,,/{asm.FullName};component/script/{script}", UriKind.RelativeOrAbsolute));

            using (var reader = new StreamReader(res.Stream)) 
            using (var cmd = m_cnn.CreateCommand())
            {
                cmd.CommandText = reader.ReadToEnd();
                cmd.ExecuteNonQuery();
            }
            
        }
    }
}
