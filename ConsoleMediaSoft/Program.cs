﻿using System;
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
            Console.WriteLine("Welcome to Elementi App..\nChoose input type: 1 - keyboard 2 - JSON");
            try
            {
                int option = int.Parse(Console.ReadLine());
                List<ElementP> elements = new List<ElementP>();
                switch (option)
                {
                    case 1:
                        Console.WriteLine("Insert desired number of elements for ElementP (higher than 0), then for ElementC (higher than -1): ");

                        int n = int.Parse(Console.ReadLine());
                        int k = int.Parse(Console.ReadLine());

                        elements = GenerateElements(n, k);

                        //Console.WriteLine("To access an element insert its id code: (-1 for exit)");
                        //int kod = int.Parse(Console.ReadLine());
                        //if (kod > 0)
                        //{
                        //    ElementP el = FindByID(kod, elements);
                        //    if (el != null) el.PrintElement();
                        //}

                        Console.WriteLine("To find all elements that has sum of subelements higher than value, please insert the value."
                            + "\n-1 for exit: ");
                        int kod = int.Parse(Console.ReadLine());
                        if(kod > 0)
                        {
                            List<ElementP> founded = FindByValue(kod, elements);
                            if (founded.Count > 0)
                            {
                                PostAsync(founded);

                                //Console.WriteLine("Desired output? DB/JSON");
                                //string output = Console.ReadLine();
                                //SaveToDB(founded, output);
                                //SaveToJSON(founded);
                            }
                        }
                        break;

                    case 2:
                        Console.WriteLine("Unesite putanju do fajla: ");

                        string url = Console.ReadLine();
                        string text = System.IO.File.ReadAllText(@url);

                        Unos.Unos json = JsonConvert.DeserializeObject<Unos.Unos>(text);
                        elements = GenerateElements(int.Parse(json.N), int.Parse(json.M));

                        Console.WriteLine("To access an element insert its id code: (-1 for exit)");
                        kod = int.Parse(Console.ReadLine());
                        if (kod > 0)
                        {
                            ElementP el = FindByID(kod, elements);
                            if (el != null) el.PrintElement();
                        }
                        break;
                    case 3:
                        Console.WriteLine("Insert date[format dd/mm/yyyy]: ");
                        string date = Console.ReadLine();
                        Console.WriteLine("Insert time[format hh:mi]: ");
                        string time = Console.ReadLine();
                        url = $"https://localhost:5001/api/values?date=" + date + "&time=" + time;
                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                        (
                           delegate { return true; }
                        );
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        var request = (HttpWebRequest)WebRequest.Create(url);
                        
                        var response = (HttpWebResponse)request.GetResponse();

                        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        Console.WriteLine(responseString);

                        break;
                    default:
                        Console.WriteLine("You've inserted wrong number!");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static List<ElementP> GenerateElements(int n, int k)
        {
            List<ElementP> array = new List<ElementP>();
            if (n > 0 && k > -1)
            {
                for (int i = 0; i < n; i++)
                {
                    array.Add(new ElementP(array.Count));
                    for (int j = 0; j < k; j++)
                    {
                        array[array.Count - 1].AddElement(new ElementC());
                    }
                }
            }
            Console.WriteLine("Elements successfully generated!");
            return array;
        }

        private static ElementP FindByID(int id, List<ElementP> array)
        {
            if (id < 0) Console.WriteLine("Exiting..");
            else
            {
                int i = 0;
                while (i < array.Count && array[i].IdentifikacioniKod != id) i++;

                if (i < array.Count) return array[i];
                else Console.WriteLine("There is no element with that id. Exiting...");
            }
            return null;
        }

        private static List<ElementP> FindByValue(int value, List<ElementP> array)
        {
            List<ElementP> foundedElements = new List<ElementP>();
            foreach(ElementP el in array)
            {
                int suma = 0;
                foreach(ElementC ec in el.Elementi)
                {
                    suma += ec.Vrednost;
                }
                if (suma > value) foundedElements.Add(el);
            }
            return foundedElements;
        }

        private static void SaveToDB(List<ElementP> array, string output)
        {
            if (output == "DB")
            {
                string conString = "DATA SOURCE=gislab-oracle.elfak.ni.ac.rs:1521/SBP_PDB;PERSIST SECURITY INFO=True;USER ID=S15315;Password=S15315";
                OracleConnection con = new OracleConnection();
                con.ConnectionString = conString;
                con.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                int i = 0;
                foreach (ElementP ep in array)
                {
                    string now = DateTime.Now.ToString();
                    string dateAndTime = now.Split(' ')[0] + " ";
                    dateAndTime += now.Split(' ')[1];

                    cmd.CommandText = $"insert into elementp (identifikacioni_kod, redni_broj, vreme_pretrage) values ({ep.IdentifikacioniKod + i}, {ep.RedniBroj}, to_date('{dateAndTime}', 'dd/mm/yyyy hh:mi:ss'))";
                    OracleDataReader odr = cmd.ExecuteReader();
                    foreach(ElementC ec in ep.Elementi)
                    {
                        cmd.CommandText = $"insert into elementc (vrednost, grupa, idkod) values ({ec.Vrednost}, '{ec.Grupa.ToString()[0]}', {ep.IdentifikacioniKod + i})";
                        odr = cmd.ExecuteReader();
                    }
                    i++;
                }
                con.Close();
            }
        }

        private static void SaveToJSON(List<ElementP> array)
        {
            List<Unos.Rootobject> list = new List<Unos.Rootobject>();
            foreach(ElementP ep in array)
            {
                list.Add(new Unos.Rootobject()
                {
                    identifikacioni_kod = ep.IdentifikacioniKod,
                    redni_broj = ep.RedniBroj,
                    vreme_pretrage = DateTime.Now.ToString(),
                    elementi = GetSubElementsC(ep.Elementi)
                }); 
            }

            string json = JsonConvert.SerializeObject(list, Formatting.Indented);

            //write string to file
            File.WriteAllText(@"c:\izlaz.json", json);
        }

        private static List<Unos.Elementi> GetSubElementsC(List<ElementC> array)
        {
            List<Unos.Elementi> list = new List<Unos.Elementi>();

            foreach(ElementC ec in array)
            {
                list.Add(new Unos.Elementi()
                {
                    grupa = ec.Grupa,
                    vrednost = ec.Vrednost
                });
            }

            return list;
        }

        private static List<Unos.Rootobject> GetSubElementsP(List<ElementP> array)
        {
            List<Unos.Rootobject> list = new List<Unos.Rootobject>();

            foreach (ElementP ep in array)
            {
                list.Add(new Unos.Rootobject()
                {
                    identifikacioni_kod = ep.IdentifikacioniKod,
                    redni_broj = ep.RedniBroj,
                    elementi = GetSubElementsC(ep.Elementi)
                });
            }

            return list;
        }

        static void PostAsync(List<ElementP> array)
        {
            using (var client = new HttpClient())
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                   delegate { return true; }
                );
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://localhost:5001/api/values");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    Unos.Root obj = new Unos.Root()
                    {
                        elementi = GetSubElementsP(array)
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