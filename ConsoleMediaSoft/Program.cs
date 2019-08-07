using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleMediaSoft.Elementi;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Net.Security;
using System.Collections.Specialized;

namespace ConsoleMediaSoft
{
    class Program
    {
        static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            // Get instance of the application
            App app = App.Instance;
            // Start program
            app.Run();
        }
    }
}