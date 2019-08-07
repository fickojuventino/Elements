using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMediaSoft.Unos
{
    public class Unos
    {
        public string N { get; set; }
        public string M { get; set; }
    }

    public class Root
    {
        public string output { get; set; }
        public List<Rootobject> elementi { get; set; }
    }

    public class Rootobject
    {
        public string identifikacioni_kod { get; set; }
        public string redni_broj { get; set; }
        public string vreme_pretrage { get; set; }
        public List<Elementi> elementi { get; set; }
    }

    public class Elementi
    {
        public char grupa { get; set; }
        public string vrednost { get; set; }
    }
}


