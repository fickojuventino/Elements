using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMediaSoft.Elementi
{
    public class ElementC
    {
        private char _grupa;
        private int _vrednost;

        public ElementC()
        {
            Random rand = new Random();
            _grupa = Convert.ToChar(rand.Next(97, 123)); // a-z malo latinicno
            _vrednost = rand.Next(1, 10); // celobrojna vrednost 1-10
        }

        public char Grupa
        {
            get { return _grupa; }
        }
        public int Vrednost
        {
            get { return _vrednost; }
        }

        #region TestMethods
        public void PrintElement()
        {
            Console.WriteLine("ELEMENTC: grupa -> " + _grupa + ", vrednost -> " + _vrednost);
        }
        #endregion
    }
}
