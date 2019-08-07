using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMediaSoft.Ulaz_Izlaz
{
    public interface IOutput
    {
        // saves data to DataBase/JSON
        void Save(List<Unos.Rootobject> array);
    }
}
