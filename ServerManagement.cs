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



        //登录方法
        //参数1.Name 用户名
        //参数2.Password 密码
        public async Task<bool> HttpClientDoPostLogin(string Name, string Password)
        {
            using (var client = new HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("username", Name));
                values.Add(new KeyValuePair<string, string>("password", Password));
                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync(REMOTE_URL + "/login", content);

                //获取登陆头部信息
                //var Headers = responsea.Headers.ToString();             
                //获得["ACCESS-TOKEN"]

                //获取登陆后主体信息
                //var responseString = await responsea.Content.ReadAsStringAsync();             
                var responseString = await response.Content.ReadAsStringAsync();
                JObject obj = (JObject)JsonConvert.DeserializeObject(responseString);
                if (obj.GetValue("success").ToString() == "True")
                {

                    family_ACCESS_TOKEN = ((System.String[])response.Headers.GetValues("ACCESS-TOKEN"))[0].ToString();
                    roleName = obj.GetValue("obj")["roles"][0]["roleName"].ToString();
                    MainWindow.LoginState = false;
                    return true;
                }
                else
                {
                    return false;
                }
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
                var response = await client.PostAsync(ServerManagement.REMOTE_URL + "/logout", content);
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



        //上传方法
        //ACCESS-TOKEN 登录返回参数
        //version 版本名称
        //file  文件
        //MD5 上传相同文件带旧文件MD5码
        public void ClanUploadingInfo(string filePath)
        {
            //postman 代码（待改）
            //测试指定文件路径

            //string fileName = "1.0.1";
            //var client = new RestClient(REMOTE_URL + "/file/upload");
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("Postman-Token", "6bc4f905-d3ae-2481-2906-9c97b56da90e");
            //request.AddHeader("Cache-Control", "no-cache");
            //request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
            //request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", "------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"ACCESS-TOKEN\"\r\n\r\n" + family_ACCESS_TOKEN + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"version\"\r\n\r\n" + fileName + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"file\"; filename=\"" + filePath + "\r\nContent-Type: text/plain\r\n\r\n\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"MD5\"\r\n\r\n8140140d8a1dbba696870f62558e26b7\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--", ParameterType.RequestBody);
            //IRestResponse response = client.Execute(request);
            //var obj = (JObject)JsonConvert.DeserializeObject(response.Content);
            ////获取MD5码
            //string mD5 = obj.GetValue("obj")["md5"].ToString();

            var client = new RestClient("http://114.113.234.159:8074/bim/revit/revit/file/upload");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Postman-Token", "bfd0adb9-7ac2-e958-4ea3-d70327e640ab");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
            request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", "------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"ACCESS-TOKEN\"\r\n\r\nBIM eyJhbGciOiJIUzUxMiJ9.eyJBQ0NFU1MtVE9LRU4iOnsidXNlcklkIjoxLCJ0b2tlbklkIjoiYzQ4YWViMzMtOWJkOC00ZmZjLWE4YzktZTY1NTRiYzllNjM2In0sImV4cCI6MTUxNDM2MTkxN30.cD10aUm2tCmOlGUi4V55Bh5_7_piAP1CKXzZR0vS_QRFKNkSviDo-UtwcOicSVWOc8wMWIV9GNQyE7wFSTEjKw\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"version\"\r\n\r\n1.0.1\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"file\"; filename=\"E:\\ceshi.txt\"\r\nContent-Type: text/plain\r\n\r\n\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"MD5\"\r\n\r\n8140140d8a1dbba696870f62558e26b7\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            var obj = (JObject)JsonConvert.DeserializeObject(response.Content);
            //获取MD5码
            string mD5 = obj.GetValue("obj")["md5"].ToString();

        }


        //下载方法
        public void DownloadAsync()
        {
            using (var client = new HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("ACCESS-TOKEN", family_ACCESS_TOKEN));
                var content = new FormUrlEncodedContent(values);
                //var response = client.PostAsync(REMOTE_URL + "/file/download/7b35b266de9c17f743e70c993b40ea26", content).Result;
                var response = client.PostAsync(REMOTE_URL + "/file/download/ba03215f3a0d0fe9278cb76620f8275d", content).Result; 
                if (response.IsSuccessStatusCode)
                {
                    using (FileStream fs = File.Create(@"C:\wenjian1.txt"))
                    {
                        Stream streamFromService = response.Content.ReadAsStreamAsync().Result;
                        streamFromService.CopyTo(fs);
                    }
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
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }

        //上传方法测试
        public void upload()
        {
            FileStream fs = new FileStream("E:/uploadCeshi.txt", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            string xmlContent = "";
            string str = sr.ReadLine();
            while (str != null)
            {
                Console.WriteLine(str);
                xmlContent = xmlContent += str;
                xmlContent += "\r\n ";
                str = sr.ReadLine();
            }

            sr.Close();
            fs.Close();

            string boundary = "Boundary-b1ed-4060-99b9-fca7ff59c113"; //Could be any string
            string Enter = "\r\n";

            //part 1
            string part1 = "--" + boundary + Enter
                    + "Content-Type: application/octet-stream" + Enter
                    + "Content-Disposition: form-data; filename=\"" + "dog.xml" + "\"; name=\"file\"" + Enter + Enter;
            //part 2
            string part2 = Enter
                    + "--" + boundary + Enter
                    + "Content-Type: text/plain" + Enter
                    + "Content-Disposition: form-data; name=\"ACCESS-TOKEN\"" + Enter + Enter
                    + family_ACCESS_TOKEN + Enter
                    + "--" + boundary + "--";
            //part 2
            string part3 = Enter
                    + "--" + boundary + Enter
                    + "Content-Type: text/plain" + Enter
                    + "Content-Disposition: form-data; name=\"version\"" + Enter + Enter
                    + "1.1.0"+ Enter
                    + "--" + boundary + "--";
            string part4 = Enter
                    + "--" + boundary + Enter
                    + "Content-Type: text/plain" + Enter
                    + "Content-Disposition: form-data; name=\"MD5\"" + Enter + Enter
                    + "b0962511388adcfff602fea59c3a3998" + Enter
                    + "--" + boundary + "--";

            //b0962511388adcfff602fea59c3a3998 MD5码

            string postDataStr = part1 + xmlContent + part2+xmlContent+part3+xmlContent+part4;
          

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://114.113.234.159:8074/bim/revit/revit/file/upload");
            request.Method = "POST";
            request.ContentType = "multipart/form-data;boundary=" + boundary;
           
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("UTF-8"));

            myStreamWriter.Write(postDataStr);

            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            

            myStreamReader.Close();
            myResponseStream.Close();


        }



        //获取文件集合对应的ByteArrayContent集合  
        private List<ByteArrayContent> GetFileByteArrayContent(HashSet<string> files)
        {
            List<ByteArrayContent> list = new List<ByteArrayContent>();
            foreach (var file in files)
            {
                var fileContent = new ByteArrayContent(File.ReadAllBytes(file));
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = Path.GetFileName(file)
                };
                list.Add(fileContent);
            }
            return list;
        }


    }
    }

