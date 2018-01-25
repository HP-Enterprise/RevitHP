using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RevitHP
{
    public class ServerManagement
    {
        private static string s_strBimBase;
        GetMD5HashFromFile getMD5 = new GetMD5HashFromFile();
        //访问服务器地址（配置文件中）
        public static string REMOTE_URL
        {
            get
            {
                if (s_strBimBase == null)
                {
                    s_strBimBase = ConfigurationManager.AppSettings["BimSrv"];
                    if (s_strBimBase==null)
                    {
                        s_strBimBase = "http://114.113.234.159:8074";
                    }

                    //string hppath= Assembly.GetExecutingAssembly().Location;
                    //var cfg = ConfigurationManager.OpenExeConfiguration(hppath);
                    //cfg.AppSettings[""]

                }
                return $"{s_strBimBase}/bim/revit/revit";
            }
        }
        public ServerManagement()
        {

        }

        //声明私有静态ACCESS_TOKEN参数
        public static string family_ACCESS_TOKEN;
        //声明公开的用户登录名
        public static string roleName;
        public static int id=0;

        //登录方法
        //参数1.Name 用户名
        //参数2.Password 密码
        public bool HttpClientDoPostLogin(string userName, SecureString pwd)
        {     
            var client = new RestClient(REMOTE_URL + "/login");
            var request = new RestRequest(Method.POST);
            request.AddParameter("username", userName);
            var ptr = Marshal.SecureStringToGlobalAllocUnicode(pwd);
            request.AddParameter("password", Marshal.PtrToStringUni(ptr));
            Marshal.ZeroFreeGlobalAllocUnicode(ptr);

            IRestResponse response = client.Execute(request);


            if (!response.IsSuccessful)
            {
                throw response.ErrorException;
            }

            JObject obj = (JObject)JsonConvert.DeserializeObject(response.Content);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            for (int i = 0; i < response.Headers.Count; i++)
            {
                string name = response.Headers[i].Name;
                string values = response.Headers[i].Value.ToString();
                dictionary.Add(name, values);
            }
         
            if (obj.GetValue("success").ToString() == "True")
            {
                if (dictionary.ContainsKey("ACCESS-TOKEN"))
                {
                    family_ACCESS_TOKEN = dictionary["ACCESS-TOKEN"];
                }
                MainWindow.isLoginState = false;
                LoginState.rolename = obj.GetValue("obj")["roles"].Last["roleName"].ToString();
                roleName = obj.GetValue("obj")["roles"].Last["roleName"].ToString();
                id = Convert.ToInt32(obj.GetValue("obj")["roles"].Last["id"]);             
                return true;
            }
            else
            {
                return false;
            }

        }

        //注销
        public async Task<bool> HttpClientDoPostLogout()
        {
            using (var client = new HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("ACCESS-TOKEN", family_ACCESS_TOKEN));
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync(REMOTE_URL + "/logout", content);
                var responseString = await response.Content.ReadAsStringAsync();
                JObject obj = (JObject)JsonConvert.DeserializeObject(responseString);
                if (obj.HasValues)
                {
                    family_ACCESS_TOKEN = null;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //第一次上传文件
        public bool FistPush(string filepath)
        {
            return Push(filepath, null);
        }

        //带旧文件MD5码上传文件
        public bool Push(string Newfilepath, string oldfilepath)
        {
            var client = new RestClient(REMOTE_URL + "/file/push");
            var request = new RestRequest(Method.POST);
            request.AddFile("file", Newfilepath);
            request.AddHeader("ACCESS-TOKEN", family_ACCESS_TOKEN);
            if (oldfilepath != null)
            {
                string MD5 = getMD5.GetMD5Hash(oldfilepath);
                request.AddHeader("MD5", MD5);
            }
            IRestResponse response = client.Execute(request);
            int ceshistatucode = (int)response.StatusCode;
            if ((int)response.StatusCode == 204)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        //下载最新版本的文件
        public bool DownloadNew(string filepath)
        {
            try
            {
                string urlstr = REMOTE_URL + "/file/pull";
                using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
                {
                    Uri url = new Uri(urlstr);
                    HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    myHttpWebRequest.Headers.Add("MD5", "00000000000000000000000000000000");
                    myHttpWebRequest.Headers.Add("ACCESS-TOKEN", family_ACCESS_TOKEN);
                    myHttpWebRequest.Method = "GET";
                    using (HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse()) { 
                    using (Stream receiveStream = myHttpWebResponse.GetResponseStream())
                    {
                        Byte[] bytes = new Byte[100];
                        int count = receiveStream.Read(bytes, 0, 100);
                        while (count != 0)
                        {
                            fs.Write(bytes, 0, count);
                            count = receiveStream.Read(bytes, 0, 100);
                        }
                    }}
                }
                return true;

            }
            catch (Exception)
            {
                return false;
                throw;
            }



        }

        //发送下载请求，返回Status Code
        public int DownloadStatusCode(string filepath, string MD5)
        {
            string urlstr = REMOTE_URL + "/file/pull";
            FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write);
            Uri url = new Uri(urlstr);
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            myHttpWebRequest.Headers.Add("md5", MD5);
            myHttpWebRequest.Headers.Add("ACCESS-TOKEN", family_ACCESS_TOKEN);
            myHttpWebRequest.Method = "GET";
            try
            {
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                fs.Close();
                return (int)myHttpWebResponse.StatusCode;
            }
            catch (WebException ex)
            {
                fs.Close();
                var resp = ex.Response as HttpWebResponse;
                if (resp != null)
                {
                    int statuscode = (int)resp.StatusCode;
                    return statuscode;
                }
                else
                {
                    return 404;
                }
            }

        }     
    }
}

