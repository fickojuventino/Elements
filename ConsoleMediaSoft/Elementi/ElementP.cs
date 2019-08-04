using ConsoleMediaSoft.Elementi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMediaSoft
{
    public class ElementP
    {
        private int _redniBroj;
        private int _identifikacioniKod;
        private List<ElementC> _elementi;

        public ElementP(int rb)
        {
            _elementi = new List<ElementC>();
            _identifikacioniKod = 1000; // to-do: generate idetifikacioniKod
            _redniBroj = rb;
        }

        public int RedniBroj
        {
            get { return _redniBroj; }
            set { _redniBroj = value; } // ukoliko treba da se azurira 
        }
        public int IdentifikacioniKod
        {
            get { return _identifikacioniKod; }
        }
        public List<ElementC> Elementi
        {
            get { return _elementi; }
        }
        public void AddElement(ElementC ec)
        {
            try
            {
                _elementi.Add(ec);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        #region TestMethods
        public void PrintElement()
        {
            Console.WriteLine("ELEMENTP: redniBroj -> " + _redniBroj + ", identifikacioniKod -> " + _identifikacioniKod);
            if (_elementi.Count > 0)
            {
                Console.WriteLine("Elementi: ");
                foreach (ElementC ec in _elementi)
                    ec.PrintElement();
            }
        }
        #endregion
    }
}
