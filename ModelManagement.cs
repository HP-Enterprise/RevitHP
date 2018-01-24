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

        public bool IsModelupload(string filepath)
        {         
            var client = new RestClient(REMOTE_URL + "/file/upload");
            var request = new RestRequest(Method.POST);
            request.AddFile("file", filepath);
            request.AddHeader("ACCESS-TOKEN", ServerManagement.family_ACCESS_TOKEN);
            string MD5 = hashFromFile.GetMD5Hash(filepath);
            request.AddHeader("MD5", MD5);
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

        public bool ModelDownload(string filepath, string MD5)
        {
            try
            {
                string urlstr = REMOTE_URL + "/file/download";
                using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
                {
                    Uri url = new Uri(urlstr);
                    HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    myHttpWebRequest.Headers.Add("MD5",MD5);                          
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
            catch (Exception)
            {

                return false;
            }
        }

        //模型删除
        public bool IsModelDelete(string MD5)
        {
            var client = new RestClient(REMOTE_URL + "/file/delete");
            var request = new RestRequest(Method.GET);
            request.AddHeader("ACCESS-TOKEN", ServerManagement.family_ACCESS_TOKEN);
            request.AddHeader("MD5",MD5);
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

        public List<Model> ModelFileList()
        {
            var client = new RestClient(REMOTE_URL + "/file/fileList");
            var request = new RestRequest(Method.GET);
            request.AddHeader("ACCESS-TOKEN", ServerManagement.family_ACCESS_TOKEN);
            IRestResponse response = client.Execute(request);
            JObject obj = (JObject)JsonConvert.DeserializeObject(response.Content);
            List<Model> list = new List<Model>();
            for (int i = 0; i < obj.GetValue("obj").Count(); i++)
            {
                Model model = new Model();
                model.Mod_Name = obj.GetValue("obj")[i]["name"].ToString();
                model.Mod_Size = obj.GetValue("obj")[i]["size"].ToString();
                model.Id =Convert.ToInt32(obj.GetValue("obj")[i]["id"]);
                model.MD5 = obj.GetValue("obj")[i]["md5"].ToString();
                list.Add(model);
            }
            return list;
        }

    }
}
