using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compresion.Interfaces;

namespace Compresion.Clases
{
    public class Nodo<T> : IComparable<T>
    {
        public T value { get; set; }

        public Nodo<T> left { get; set; }

        public Nodo<T> right { get; set; }

        private ComparadorNodosDelegate<T> comparador;

        public Nodo(T _value, ComparadorNodosDelegate<T> _comparador)
        {
            this.value = _value;
            this.left = null;
            this.right = null;
            comparador = _comparador;
        }

        public int CompareTo(T _other)
        {
            return comparador(this.value, _other);
        }
    }
}
