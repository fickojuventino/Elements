using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMediaSoft.Elementi
{
    public class ElementC : IElement
    {
        private char _grupa;
        private int _vrednost;

        private static readonly RNGCryptoServiceProvider _generator = new RNGCryptoServiceProvider();

        public ElementC()
        {
            Random rand = new Random();
            _grupa = Convert.ToChar(Between(97, 122)); // a-z malo latinicno
            _vrednost = Between(1, 9); // celobrojna vrednost 1-9
        }

        public static int Between(int minimumValue, int maximumValue)
        {
            byte[] randomNumber = new byte[1];

            _generator.GetBytes(randomNumber);

            double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);

            // We are using Math.Max, and substracting 0.00000000001, 
            // to ensure "multiplier" will always be between 0.0 and .99999999999
            // Otherwise, it's possible for it to be "1", which causes problems in our rounding.
            double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);

            // We need to add one to the range, to allow for the rounding done with Math.Floor
            int range = maximumValue - minimumValue + 1;

            double randomValueInRange = Math.Floor(multiplier * range);

            return (int)(minimumValue + randomValueInRange);
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
