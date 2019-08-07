using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMediaSoft.Ulaz_Izlaz
{
    public interface IInput
    {
        // returns list of parameters
        List<int> Read();
    }
}
