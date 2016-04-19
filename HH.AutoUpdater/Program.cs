using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HH.AutoUpdater
{
    class Program
    {

        /// <summary>
        /// Заготовка
        /// </summary>
        static async void getContent()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.hh.ru");
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("hhApp");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer GC1MMBEOQVSPP0AFPGC2OCCJK465T3NL5IIHE0IMRPKVBR35IA17N0SM5D0K3IVO");
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            HttpResponseMessage response = new HttpResponseMessage();

            response = await client.GetAsync("https://api.hh.ru/resumes/mine");
            var s = await response.Content.ReadAsStringAsync();
            Console.Write(s);
            Console.ReadKey();

        }

        
        static int Main(string[] args)
        {

            var Options = new Dictionary<string, string>();
            
            var option_name = "";

            foreach(var arg in args)
            {
                if (arg.StartsWith("-"))
                    option_name = arg.Substring(1);
                else
                    Options.Add(option_name, arg);                   
            }
            
            string user_token;

            if(!Options.TryGetValue("token", out user_token))    
            {
                Console.WriteLine("Не указан токен пользователя для аутентификации");
                return 0x1;
            }

            Uri address = new Uri("https://api.hh.ru/resumes/mine");

            HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.Create(address);

            wr.Credentials = CredentialCache.DefaultCredentials;
            wr.UseDefaultCredentials = false;
            wr.Headers[HttpRequestHeader.Authorization] = "Bearer " + user_token;
            wr.Accept = "*/*";
            //  wr.Proxy = new WebProxy("127.0.0.1:8888");

            wr.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64)";
            wr.AllowAutoRedirect = true;
            wr.KeepAlive = true;
            wr.Host = "api.hh.ru";
            wr.Method = "GET";
            wr.PreAuthenticate = false;

            List<string> cv_ids = new List<string>();
            HttpWebResponse wre = null;
            try
            {
                wre = (HttpWebResponse)wr.GetResponse();

            }
            catch (WebException e)
            {
                var d = e.Response;
            }

            StreamReader sr = new StreamReader(wre.GetResponseStream());
            string s = sr.ReadToEnd();

            JObject jo = JObject.Parse(s);
            IList<JToken> jtoks = jo["items"].Children().ToList();

            foreach (var cv in jtoks)
                cv_ids.Add(cv["id"].ToString());


            int i = 0;

            foreach (var id in cv_ids)
            {
                HttpWebRequest wry = (HttpWebRequest)HttpWebRequest.Create(new Uri("https://api.hh.ru/resumes/" + id + "/publish"));

                wry.Credentials = CredentialCache.DefaultCredentials;
                wry.UseDefaultCredentials = false;
                wry.Headers[HttpRequestHeader.Authorization] = "Bearer GC1MMBEOQVSPP0AFPGC2OCCJK465T3NL5IIHE0IMRPKVBR35IA17N0SM5D0K3IVO";
                wry.Accept = "*/*";
                //  wr.Proxy = new WebProxy("127.0.0.1:8888");
                wr.PreAuthenticate = false;
                wry.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64)";
                wry.AllowAutoRedirect = true;
                wry.KeepAlive = true;
                wry.Host = "api.hh.ru";
                wry.Method = "POST";
                //wry.ContentType = "application/x-www-form-urlencoded";

                BinaryWriter bwr = new BinaryWriter(wry.GetRequestStream());
                bwr.Write(jtoks[i].ToString());
                bwr.Close();
                try
                {
                    wre = (HttpWebResponse)wry.GetResponse();
                }
                catch (WebException e)
                {
                    var resp = (HttpWebResponse)e.Response;
                    Console.WriteLine(resp.StatusCode.ToString() + " " + resp.StatusDescription);
                    continue;
                }
                finally
                {
                    i++;
                }

                Console.WriteLine(wre.StatusCode.ToString() + " " + wre.StatusDescription);
                if (wre.StatusCode == HttpStatusCode.NoContent)
                    Console.WriteLine("Резюме " + id + " обновлено");
                else
                    Console.WriteLine("Резюме " + id + " не обновлено");
            }
            return 0x0;
        }

    }
}
