using Insus.NET;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RevitHP
{
    public class ServerManagement
    {
        // local http://114.113.234.159:8074/bim/revit/
        // remote http://114.113.234.159:8074/bim/revit

        public static string  REMOTE_URL= "http://114.113.234.159:8074/bim/revit/revit";
      
        //声明公开静态ACCESS_TOKEN参数
        public static string family_ACCESS_TOKEN;
        //登录方法
        //参数1.Name 用户名
        //参数2.Password 密码
        public async Task<JObject> HttpClientDoPostLogin(string Name, string Password)
        {
            using (var client = new HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("username", "admin"));
                values.Add(new KeyValuePair<string, string>("password", "admin"));
                var content = new FormUrlEncodedContent(values);
              
                var response = await client.PostAsync(REMOTE_URL+"/login", content);
                //获取登陆头部信息
                //var Headers = responsea.Headers.ToString();             
                //获得["ACCESS-TOKEN"]
                family_ACCESS_TOKEN = ((System.String[])response.Headers.GetValues("ACCESS-TOKEN"))[0].ToString();
                //获取登陆后主体信息
                //var responseString = await responsea.Content.ReadAsStringAsync();             
                var responseString = await response.Content.ReadAsStringAsync();
                JObject obj = (JObject)JsonConvert.DeserializeObject(responseString);
                return obj;
            }
        }



        //上传方法
        //ACCESS-TOKEN 登录返回参数
        //version 版本名称
        //file  文件
        //MD5 上传相同文件带旧文件MD5码
        public void ClanUploadingInfo(string ACCESS_TOKEN,string filePath)
        {
            string ceshiPath = "F:\\经济头脑.png";
            string fileName = "1.0.27";
            ////var client = new RestClient(REMOTE_URL+"/file/upload");
            //var client = new RestClient(REMOTE_URL+ "/file/upload");
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("Postman-Token", "a279ed3d-968f-3b7a-84e0-534040c070fd");
            //request.AddHeader("Cache-Control", "no-cache");
            //request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
            //request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", "------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"ACCESS-TOKEN\"\r\n\r\n"+ACCESS_TOKEN+"\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"version\n\"\r\n\r\n"+ fileName + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"file\"; filename=\"" + ceshiPath + "\r\nContent-Type: text/plain\r\n\r\n\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--", ParameterType.RequestBody);
            //IRestResponse response = client.Execute(request);

            var client = new RestClient(REMOTE_URL+"/file/upload");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Postman-Token", "c80bd569-f2fa-b699-5e20-6d45d5cb32d8");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
            request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", "------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"ACCESS-TOKEN\"\r\n\r\n"+ACCESS_TOKEN+"\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"version\"\r\n\r\n"+ fileName+ "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"file\"; filename=\""+ ceshiPath +"\r\nContent-Type: text/plain\r\n\r\n\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"MD5\"\r\n\r\nb91d715eecceb19686b6560fa23b6a4d\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            var obj = (JObject)JsonConvert.DeserializeObject(response.Content);
            //获取MD5码
            string mD5 = obj.GetValue("obj")["md5"].ToString();
        }
 
        
        //下载方法
        public void DownloadAsync(string ACCESS_TOKEN)
        {
            using (var client = new HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("ACCESS-TOKEN", ACCESS_TOKEN));              
                var content = new FormUrlEncodedContent(values);             
                var response = client.PostAsync(REMOTE_URL+ "/file/download/189d1769de111b7a17012e4ba7eb56f4", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    using (FileStream fs = File.Create(@"C:\1.png"))
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

    }
}
