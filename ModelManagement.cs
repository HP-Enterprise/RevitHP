using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitHP
{
    class ModelManagement
    {
        GetMD5HashFromFile hashFromFile = new GetMD5HashFromFile();
        private static string s_strBimBase;

        private static string address = "http://1411018008.tunnel.echomod.cn/revit/file";

        //访问服务器地址（配置文件中）
        public static string REMOTE_URL
        {
            get
            {
                if (s_strBimBase == null)
                {
                    s_strBimBase = ConfigurationManager.AppSettings["BimSrv"];
                }
                return $"{s_strBimBase}/bim/revit/revit";
            }
        }

        public bool Modelupload(string filepath)
        {
            var client = new RestClient($"{address}/upload");
            var request = new RestRequest(Method.POST);
            request.AddFile("file", filepath);
            request.AddHeader("ACCESS-TOKEN", ServerManagement.family_ACCESS_TOKEN);
            string MD5 = hashFromFile.GetMD5Hash(filepath);
            request.AddHeader("MD5", MD5);
            IRestResponse response = client.Execute(request);


            //int ceshistatucode = (int)response.StatusCode;
            return true;

        }

        public bool ModelDownload(string filepath, string MD5)
        {
            var client = new RestClient($"{address}/download");
            var request = new RestRequest(Method.GET);
            request.AddHeader("ACCESS-TOKEN", ServerManagement.family_ACCESS_TOKEN);
            request.AddHeader("MD5", MD5);
            IRestResponse response = client.Execute(request);
            return true;
        }

        public bool ModelDelete(string MD5)
        {
            var client = new RestClient($"{address}/delete");
            var request = new RestRequest(Method.GET);
            request.AddHeader("ACCESS-TOKEN", ServerManagement.family_ACCESS_TOKEN);
            request.AddHeader("MD5", MD5);
            IRestResponse response = client.Execute(request);
            return true;
        }

        public List<string> ModelFileList()
        {
            var client = new RestClient($"{address}/fileList");
            var request = new RestRequest(Method.GET);
            request.AddHeader("ACCESS-TOKEN", ServerManagement.family_ACCESS_TOKEN);
            IRestResponse response = client.Execute(request);
            List<string> list = new List<string>();
            return list;

        }
    }
}
