using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMediaSoft.Ulaz_Izlaz
{
    public class API : IOutput
    {
        public void Save(List<Unos.Rootobject> array)
        {
            // if the type implements IDisposable, it automatically disposes it
            using (var client = new HttpClient())
            {
                // removes SSL certificate validation
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                   delegate { return true; }
                );
                // selects the version of SSL/TLS to use for new connections
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://localhost:5001/api/values");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    Unos.Root obj = new Unos.Root()
                    {
                        elementi = array
                    };
                    string json = JsonConvert.SerializeObject(obj, Formatting.Indented);

                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }
            }
        }
    }
}
