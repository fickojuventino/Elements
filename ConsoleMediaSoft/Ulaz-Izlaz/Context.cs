using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMediaSoft.Ulaz_Izlaz
{
    // strategy pattern
    public class Context
    {
        private IInput _input;
        private IOutput _output;

        public Context() { }
        public IInput Input {
            get { return _input; }
            set { _input = value; }
        }
        public IOutput Output
        {
            get { return _output; }
            set { _output = value; }
        }
    }
}
