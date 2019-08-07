using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMediaSoft.Ulaz_Izlaz
{
    public class Keyboard : IInput
    {
        public List<int> Read()
        {
            Console.WriteLine("\nInsert desired number of elements for ElementP (higher than 0),\n then for ElementC (higher than -1): ");
            List<int> parameters = new List<int>();
            parameters.Add(int.Parse(Console.ReadLine()));
            parameters.Add(int.Parse(Console.ReadLine()));
            return parameters;
        }

        public Keyboard()
        {
        }
    }
}
