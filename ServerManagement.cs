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
        public ServerManagement()
        {

        }


        //声明私有静态ACCESS_TOKEN参数
        private static string family_ACCESS_TOKEN;
        //声明公开的用户登录名
        public static string roleName;
        public static int id;


        //登录方法
        //参数1.Name 用户名
        //参数2.Password 密码
        public bool HttpClientDoPostLogin(string userName, SecureString pwd)
        {
            //using (var client = new HttpClient())
            //{
            //    var values = new List<KeyValuePair<string, string>>();
            //    values.Add(new KeyValuePair<string, string>("username", Name));
            //    values.Add(new KeyValuePair<string, string>("password", Password));
            //    var content = new FormUrlEncodedContent(values);
            //    var response = await client.PostAsync(REMOTE_URL + "/login", content);
            //    //var response = await client.PostAsync("http://1411018008.tunnel.echomod.cn/revit/login", content);
            //    var responseString = await response.Content.ReadAsStringAsync();
            //    JObject obj = (JObject)JsonConvert.DeserializeObject(responseString);
            //    if (obj.GetValue("success").ToString() == "True")
            //    {
            //        family_ACCESS_TOKEN = ((System.String[])response.Headers.GetValues("ACCESS-TOKEN"))[0].ToString();
            //        roleName = obj.GetValue("obj")["roles"][0]["roleName"].ToString();
            //        MainWindow.LoginState = false;
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
          
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
            Dictionary<string,string> dictionary = new Dictionary<string,string>();
            var dicheaders = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < response.Headers.Count; i++)
            {
                string name = response.Headers[i].Name;
                string values = response.Headers[i].Value.ToString();
                //dicheaders.Add(new KeyValuePair<string, string>(name, values));
                dictionary.Add(name, values);
            }
            //if (dictionary.ContainsKey("ACCESS-TOKEN"))
            //{
            //    family_ACCESS_TOKEN = dictionary["ACCESS-TOKEN"];
                
            //}
            //else
            //{

            //}


             //LoginVM loginVM = new LoginVM();
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
                
                //loginVM.RoleName= obj.GetValue("obj")["roles"].Last["roleName"].ToString();
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
            var client = new RestClient(REMOTE_URL + "/file/push");        
            var request = new RestRequest(Method.POST);
            request.AddFile("file", filepath);
            request.AddHeader("ACCESS-TOKEN", family_ACCESS_TOKEN);
            IRestResponse response = client.Execute(request);
            int status = (int)response.StatusCode;
            if ((int)response.StatusCode == 204)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //带旧文件MD5码上传文件
        public bool Push(string Newfilepath, string oldfilepath)
        {       
            var client = new RestClient(REMOTE_URL + "/file/push");
            var request = new RestRequest(Method.POST);
            request.AddFile("file", Newfilepath);
            request.AddHeader("ACCESS-TOKEN", family_ACCESS_TOKEN);
            string MD5 = GetMD5HashFromFile(oldfilepath);
            request.AddHeader("MD5", MD5);
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
                FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write);
                Uri url = new Uri(urlstr);
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                myHttpWebRequest.Headers.Add("MD5", "00000000000000000000000000000000");
                myHttpWebRequest.Headers.Add("ACCESS-TOKEN", family_ACCESS_TOKEN);
                myHttpWebRequest.Method = "GET";
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                Stream receiveStream = myHttpWebResponse.GetResponseStream();
                Byte[] bytes = new Byte[100];
                int count = receiveStream.Read(bytes, 0, 100);
                while (count != 0)
                {
                    fs.Write(bytes, 0, count);
                    count = receiveStream.Read(bytes, 0, 100);
                }
                fs.Close();
                receiveStream.Close();
                myHttpWebResponse.Close();
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

        /// <summary>  
        /// 获取文件的MD5码  
        /// </summary>  
        /// <param name="fileName">传入的文件名（含路径及后缀名）</param>  
        /// <returns></returns>  
        public string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, System.IO.FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception)
            {
                //如果发生异常，返回一个错误的MD5码
                return "1111111111111111111111111111111";
            }
        }
    }
}

