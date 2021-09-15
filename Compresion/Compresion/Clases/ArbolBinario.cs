using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compresion.Interfaces;

namespace Compresion.Clases
{
    public class ArbolBinario<T> : IArbolBinario<T>
    {

        private Nodo<T> _raiz;

        public ArbolBinario()
        {
            _raiz = null;
        }

        public void Eliminar(T _key)
        {
            throw new NotImplementedException();
        }

        public void Insertar(Nodo<T> _nuevo)
        {
            if (_raiz == null)
            {
                _raiz = _nuevo;
            }
            else
            {
                InsercionInterna(_raiz, _nuevo);
            }
        }

        private void InsercionInterna(Nodo<T> _actual, Nodo<T> _nuevo)
        {
            if (_actual.CompareTo(_nuevo.value) < 0)
            {
                if (_actual.right == null)
                {
                    _actual.right = _nuevo;
                }
                else
                {
                    InsercionInterna(_actual.right, _nuevo);
                }
            }
            else if (_actual.CompareTo(_nuevo.value) > 0)
            {
                if (_actual.left == null)
                {
                    _actual.left = _nuevo;
                }
                else
                {
                    InsercionInterna(_actual.left, _nuevo);
                }
            }
        } //Fin de inserción interna.

        public Nodo<T> ObtenerRaiz()
        {
            return _raiz;
        }

        public void EnOrden(RecorridoDelegate<T> _recorrido)
        {
            RecorridoEnOrdenInterno(_recorrido, _raiz);
        }

        public void PostOrden(RecorridoDelegate<T> _recorrido)
        {
            RecorridoPostOrdenInterno(_recorrido, _raiz);
        }

        public void PreOrden(RecorridoDelegate<T> _recorrido)
        {
            RecorridoPreOrdenInterno(_recorrido, _raiz);
        }

        private void RecorridoEnOrdenInterno(RecorridoDelegate<T> _recorrido, Nodo<T> _actual)
        {
            if (_actual != null)
            {
                RecorridoEnOrdenInterno(_recorrido, _actual.left);

                _recorrido(_actual);

                RecorridoEnOrdenInterno(_recorrido, _actual.right);
            }
        }

        private void RecorridoPostOrdenInterno(RecorridoDelegate<T> _recorrido, Nodo<T> _actual)
        {
            if (_actual != null)
            {
                RecorridoEnOrdenInterno(_recorrido, _actual.left);

                RecorridoEnOrdenInterno(_recorrido, _actual.right);

                _recorrido(_actual);

                Console.Write(_actual.value.ToString());
            }
        }

        private void RecorridoPreOrdenInterno(RecorridoDelegate<T> _recorrido, Nodo<T> _actual)
        {
            if (_actual != null)
            {
                _recorrido(_actual);

                RecorridoEnOrdenInterno(_recorrido, _actual.left);

                RecorridoEnOrdenInterno(_recorrido, _actual.right);
            }
        }
    }
}
