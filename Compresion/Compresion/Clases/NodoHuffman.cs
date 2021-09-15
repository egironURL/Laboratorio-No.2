using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compresion.Interfaces;

namespace Compresion.Clases
{
    public class NodoHuffman
    {
        public int NoASCII { get; set; }

        public int frecuency { get; set; }

        public double probability { get; set; }

        public NodoHuffman left { get; set; }

        public NodoHuffman right { get; set; }

        public NodoHuffman(int _value, int _frecuency, double _probability)
        {
            this.NoASCII = _value;
            this.frecuency = _frecuency;
            this.probability = _probability;
            this.left = null;
            this.right = null;
        }

        public double GetProbability()
        {
            return this.probability;
        }
    }
}
