using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleMediaSoft.Unos;

namespace ConsoleMediaSoft.Ulaz_Izlaz
{
    public class JSON : IInput, IOutput
    {
        public List<int> Read()
        {
            Console.WriteLine("\nUnesite putanju do fajla: ");

            string url = Console.ReadLine();
            string text = File.ReadAllText(@url);

            // Serialization is the process of converting an object into a stream of bytes to store
            // the object or transmit it to memory, a database, or a file.
            // Its main purpose is to save the state of an object in order to be able to recreate it when needed.
            // The reverse process is called deserialization.
            Unos.Unos json = JsonConvert.DeserializeObject<Unos.Unos>(text);
            List<int> list = new List<int>();
            list.Add(int.Parse(json.N));
            list.Add(int.Parse(json.M));
            return list;
        }

        public void Save(List<Rootobject> array)
        {
            Console.WriteLine("Writing to json file..");
            string json = JsonConvert.SerializeObject(array, Formatting.Indented);

            //write string to file
            File.WriteAllText(@"c:\izlaz.json", json);
            Console.WriteLine("Writing to file successfully done. Exiting..");
        }
    }
}
