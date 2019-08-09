using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMediaSoft.Elementi
{
    public class App
    {
        private List<ElementP> _elements;
        private static App _instance = null;
        private static readonly object _padlock = new object();
        // private constructor - Singleton design pattern
        private App()
        {
            _elements = new List<ElementP>();
        }

        public static App Instance
        {
            get
            {
                // thread-safe
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new App();
                    }
                    return _instance;
                }
            }
        }
        public void Run()
        {
            Console.WriteLine("Welcome to Elementi App..\nChoose input type:\n1) Keyboard\n2) JSON\n3) Load previous searches by time"
                + "\nInsert a number before option to choose.");
            try
            {
                List<ElementP> elements = new List<ElementP>();
                Ulaz_Izlaz.Context context = new Ulaz_Izlaz.Context();

                int option = int.Parse(Console.ReadLine());
                switch (option)
                {
                    case 1:
                        // sets the input to keyboard
                        context.Input = new Ulaz_Izlaz.Keyboard();
                        List<int> parameters = new List<int>();
                        // gets parameters from input
                        parameters = context.Input.Read();
                        // generates elements
                        elements = GenerateElements(parameters.ElementAt(0), parameters.ElementAt(1));
                        break;
                    case 2:
                        // sets the input to json
                        context.Input = new Ulaz_Izlaz.JSON();
                        parameters = context.Input.Read();
                        elements = GenerateElements(parameters.ElementAt(0), parameters.ElementAt(1));
                        break;
                    case 3:
                        Console.WriteLine("Insert date[format dd/mm/yyyy]: ");
                        string date = Console.ReadLine();
                        Console.WriteLine("Insert time[format hh:mi]: ");
                        string time = Console.ReadLine();
                        // get request
                        Get(date, time);
                        return;
                    default:
                        // if user enters bad option - program ends
                        Console.WriteLine("You've inserted wrong number!");
                        return;
                }

                Console.WriteLine("1)Access an element by its id code");
                Console.WriteLine("2)Find and save all elements that has sum of subelements higher than some value"
                    + "\n3)Skip\n-1 for exit: ");
                int code = int.Parse(Console.ReadLine());
                // choosing an option
                switch (code)
                {
                    case 1:
                        Console.WriteLine("\nInsert id:");
                        code = int.Parse(Console.ReadLine());
                        if (code > 0)
                        {
                            // finds single element by its ID
                            ElementP el = FindByID(code, elements);
                            // if we got something then print it
                            if (el != null) el.PrintElement();
                        }
                        break;
                    case 2:
                        Console.WriteLine("Insert a value:");
                        code = int.Parse(Console.ReadLine());
                        if (code > 0)
                        {
                            // finds all elements that reach the condition
                            List<ElementP> founded = FindByValue(code, elements);
                            if (founded.Count > 0)
                            {
                                Console.WriteLine("Desired output? DB/JSON");
                                string output = Console.ReadLine();
                                Save(founded, output);
                            }
                        }
                        break;
                    default:
                        Console.WriteLine("\nWrong option..");
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private List<ElementP> GenerateElements(int n, int k)
        {
            List<ElementP> array = new List<ElementP>();
            // at least 1 element of type ElementP
            // can be without subelements of type ElementC
            if (n > 0 && k > -1)
                for (int i = 0; i < n; i++)
                {
                    array.Add(new ElementP(array.Count, 1000 + i));
                    for (int j = 0; j < k; j++)
                        array[array.Count - 1].AddElement(new ElementC());
                }
            Console.WriteLine("\nElements successfully generated!");
            Console.WriteLine("Elements ids start from 1000.\n");
            return array;
        }

        public ElementP FindByID(int id, List<ElementP> array)
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

        public List<ElementP> FindByValue(int value, List<ElementP> array)
        {
            List<ElementP> foundedElements = new List<ElementP>();
            foreach (ElementP el in array)
            {
                int suma = 0;
                foreach (ElementC ec in el.Elementi)
                {
                    suma += ec.Vrednost;
                }
                if (suma > value) foundedElements.Add(el);
            }
            return foundedElements;
        }

        // returns all elemenets of type ElementC in JSON form
        public List<Unos.Elementi> GetSubElementsC(List<ElementC> array)
        {
            List<Unos.Elementi> list = new List<Unos.Elementi>();

            foreach (ElementC ec in array)
            {
                list.Add(new Unos.Elementi()
                {
                    grupa = ec.Grupa,
                    vrednost = ec.Vrednost.ToString()
                });
            }

            return list;
        }

        // return all elements of type ElementP in JSON form
        public List<Unos.Rootobject> GetSubElementsP(List<ElementP> array)
        {
            List<Unos.Rootobject> list = new List<Unos.Rootobject>();

            foreach (ElementP ep in array)
            {
                list.Add(new Unos.Rootobject()
                {
                    identifikacioni_kod = ep.IdentifikacioniKod.ToString(),
                    redni_broj = ep.RedniBroj.ToString(),
                    vreme_pretrage = DateTime.Now.ToString(),
                    elementi = GetSubElementsC(ep.Elementi)
                });
            }

            return list;
        }

        // sets output type and saves to DataBase/JSON
        public void Save(List<ElementP> founded, string output)
        {
            Ulaz_Izlaz.Context context = new Ulaz_Izlaz.Context();
            if(output == "DB")
            {
                context.Output = new Ulaz_Izlaz.API();
                context.Output.Save(GetSubElementsP(founded));
            }
            else if(output == "JSON")
            {
                context.Output = new Ulaz_Izlaz.JSON();
                context.Output.Save(GetSubElementsP(founded));
            }
            return;
        }

        // sends get request to server
        private void Get(string date, string time)
        {
            // sets up the url
            string url = "https://localhost:5001/api/values?date=" + date + "&time=" + time;

            // removes SSL certificate validation
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
            (
               delegate { return true; }
            );

            // selects the version of SSL/TLS to use for new connections
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine("\n" + response.Headers);
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            Console.WriteLine(responseString);
        }
    }
}
