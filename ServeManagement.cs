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
using System.Text;
using System.Threading.Tasks;

namespace RevitHP
{
   public class ServeManagement
    {
        //注销端口链接


        //声明静态注销状态
        public static string cancel_state;
        //声明公开静态ACCESS_TOKEN参数
        public static string family_ACCESS_TOKEN;
        //登录方法
        //参数1.Name 用户名
        //参数2.Password 密码
        public async Task<JObject> HttpClientDoPostAsync(string Name, string Password)
        {
            using (var client = new HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("username", "admin"));
                values.Add(new KeyValuePair<string, string>("password", "admin"));
                var content = new FormUrlEncodedContent(values);
                //var response = await client.PostAsync("http://192.168.10.54:9007/revit/login",content);
                var response = await client.PostAsync("http://114.113.234.159:8074/bim/revit/revit/login", content);
                //获取登陆头部信息
                //var Headers = responsea.Headers.ToString();             
                //获得["ACCESS-TOKEN"]
                family_ACCESS_TOKEN=((System.String[])response.Headers.GetValues("ACCESS-TOKEN"))[0].ToString();
                //获取登陆后主体信息
                //var responseString = await responsea.Content.ReadAsStringAsync();             
                var responseString = await response.Content.ReadAsStringAsync();
                JObject obj = (JObject)JsonConvert.DeserializeObject(responseString);
                return obj;
            }
        }

        
        //注销
        //登录成功后获取参数ACCESS-TOKEN
        public async void HttpCancelDoPost(string ACCESS_TOKEN)
        {
           
            using (var client = new HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("ACCESS-TOKEN", ACCESS_TOKEN));
                //values.Add(new KeyValuePair<string, string>("password",Password));
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("http://114.113.234.159:8074/bim/revit/revit/logout", content);
                var responseString = await response.Content.ReadAsStringAsync();
                JObject obj = (JObject)JsonConvert.DeserializeObject(responseString);
                if (obj.HasValues)
                {
                    cancel_state = "true";
                }
                else
                {
                    cancel_state = "false";
                }
            }
          
        }


        //下载方法
        private async Task DownloadAsync(string ACCESS_TOKEN)
        {
            using (var client = new HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("ACCESS-TOKEN", ACCESS_TOKEN));
                //values.Add(new KeyValuePair<string, string>("password",Password));
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("http://192.168.10.54:9007/revit/file/download/31", content);

            }
        }


        //上传方法
        private void ClanUploadingInfo(string ACCESS_TOKEN)
        {
            var client = new RestClient("http://192.168.10.54:9007/revit/file/upload");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Postman-Token", "7dac66d2-ac51-8d05-e22a-87ea270c8451");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
            request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", "------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"ACCESS-TOKEN\"\r\n\r\nBIM eyJhbGciOiJIUzUxMiJ9.eyJBQ0NFU1MtVE9LRU4iOnsidXNlcklkIjoxLCJ0b2tlbklkIjoiYmIyNWQ5ODctZjk0My00OGExLWFlOWEtOWE5ZjI1OTliOWQyIn0sImV4cCI6MTUxMzU2NDE5NH0.3S1S_dy6TdcYPCMGGQ6LWlMsluE6nF5cDIiv4DMhjn4cKnrhe89jiHwyfrcsOpCWfVsQT88NtZ6l702B42ZAuQ\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"file\"; filename=\"F:\\ceshi.txt\"\r\nContent-Type: text/plain\r\n\r\n\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);


            //string str = System.Text.Encoding.Default.GetString(ConvertToBinary(path.sqlLitepath));
            //using (var client = new HttpClient())
            //{
            //    var values = new List<KeyValuePair<string, string>>();
            //    values.Add(new KeyValuePair<string, string>("ACCESS-TOKEN", ACCESS_TOKEN));
            //    values.Add(new KeyValuePair<string, byte>("file", str));
            //    //values.Add(new KeyValuePair<string, string>("password",Password));
            //    var content = new FormUrlEncodedContent(values);
            //    var response = await client.PostAsync("http://192.168.10.54:9007/revit/file/upload", content);
            //    var responseString = await response.Content.ReadAsStringAsync();
            //    JObject obj = (JObject)JsonConvert.DeserializeObject(responseString);

            //}

        }

        /// <summary>
        /// 使用HttpClient上传文件及传递参数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postParameters"></param>
        /// <param name="lstFilePath"></param>
        /// <returns></returns>
        public static async Task<string> HttpPostAsync(string url, NameValueCollection postParameters, List<string> lstFilePath = null)
        {
            //var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

            using (var httpClient = new HttpClient())
            {
                using (MultipartFormDataContent formData = new MultipartFormDataContent())
                {
                    if (lstFilePath != null)
                    {
                        for (int i = 0; i < lstFilePath.Count; i++)
                        {
                            int start = lstFilePath[i].LastIndexOf('\\');
                            string name = lstFilePath[i].Substring(start + 1);
                            var fileContent = new ByteArrayContent(File.ReadAllBytes(lstFilePath[i]));
                            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("file")
                            {
                                FileName = name
                            };
                            formData.Add(fileContent);
                        }
                    }
                    foreach (string key in postParameters.Keys)
                    {
                        formData.Add(new StringContent(postParameters[key]), key);

                    }

                    Task<HttpResponseMessage> response = httpClient.PostAsync(url, formData);
                    string tempResult = await response.Result.Content.ReadAsStringAsync();
                    return tempResult;
                }
            }
        }

    }
}
