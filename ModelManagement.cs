using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RevitHP
{
    class ModelManagement
    {
        GetMD5HashFromFile hashFromFile = new GetMD5HashFromFile();
        private static string s_strBimBase;
        //private static string address = "http://1411018008.tunnel.echomod.cn/bim/revit/revit/file";
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
            //var client = new RestClient($"{address}/upload");
            var client = new RestClient("http://1411018008.tunnel.echomod.cn/revit/file/upload");
            var request = new RestRequest(Method.POST);
            request.AddFile("file", filepath);
            request.AddHeader("ACCESS-TOKEN", ServerManagement.family_ACCESS_TOKEN);
            string MD5 = hashFromFile.GetMD5Hash(filepath);
            request.AddHeader("MD5", MD5);
            IRestResponse response = client.Execute(request);
            if (true)
            {

            }
            
            int ceshistatucode = (int)response.StatusCode;
            return true;
        }

        public bool ModelDownload(string filepath, string MD5)
        {
            string urlstr = "http://1411018008.tunnel.echomod.cn/revit/file/download";
            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                Uri url = new Uri(urlstr);
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                myHttpWebRequest.Headers.Add("MD5", "ff64011aec56c57954b751c7044a1abc");
                myHttpWebRequest.Headers.Add("ACCESS-TOKEN", ServerManagement.family_ACCESS_TOKEN);
                myHttpWebRequest.Method = "GET";
                using (HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse())
                using (Stream receiveStream = myHttpWebResponse.GetResponseStream())
                {
                    Byte[] bytes = new Byte[100];
                    int count = receiveStream.Read(bytes, 0, 100);
                    while (count != 0)
                    {
                        fs.Write(bytes, 0, count);
                        count = receiveStream.Read(bytes, 0, 100);
                    }
                }
            }
            return true;
        }




        public bool ModelDelete(string MD5)
        {
            var client = new RestClient("http://1411018008.tunnel.echomod.cn/revit/file/delete");
            var request = new RestRequest(Method.GET);
            request.AddHeader("ACCESS-TOKEN", ServerManagement.family_ACCESS_TOKEN);
            request.AddHeader("MD5", "ff64011aec56c57954b751c7044a1abc");
            IRestResponse response = client.Execute(request);
            JObject obj = (JObject)JsonConvert.DeserializeObject(response.Content);
            if (obj.GetValue("success").ToString() == "True")
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        public List<string> ModelFileList()
        {
            var client = new RestClient("http://1411018008.tunnel.echomod.cn/revit/file/fileList");
            var request = new RestRequest(Method.GET);
            request.AddHeader("ACCESS-TOKEN", ServerManagement.family_ACCESS_TOKEN);
            IRestResponse response = client.Execute(request);
            JObject obj = (JObject)JsonConvert.DeserializeObject(response.Content);
            List<string> list = new List<string>();
            for (int i = 0; i < obj.GetValue("obj").Count(); i++)
            {
                string a = obj.GetValue("obj")[i]["md5"].ToString();
                list.Add(obj.GetValue("obj")[i]["md5"].ToString());
            }
            return list;
        }

    }
}
