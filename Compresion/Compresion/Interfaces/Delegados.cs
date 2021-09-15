using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compresion.Clases;

namespace Compresion.Interfaces
{
    public delegate int ComparadorNodosDelegate<T>(T _actual, T _nuevo);

    public delegate void RecorridoDelegate<T>(Nodo<T> _acutal);
}
