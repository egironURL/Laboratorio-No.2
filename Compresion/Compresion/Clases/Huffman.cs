using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Compresion.Interfaces;

namespace Compresion.Clases
{
    public class Huffman : ICompresion
    {
        public string FileName { get; set; }

        public string PathFileHUFF { get; set; }

        public double CompressRatio { get; set; }

        public double CompressFactor { get; set; }

        public int ReductionPer { get; set; }

        public FileInfo fileData { get; set; }

        public FileInfo fileDecompress { get; set; }

        public Huffman(string _FileName, string _pathFileHuff)
        {
            this.FileName = _FileName;
            this.PathFileHUFF = _pathFileHuff;
        }

        public Huffman(FileInfo _FileName, FileInfo _FileDecompress)
        {
            this.fileData = _FileName;
            this.fileDecompress = _FileDecompress;

            if (_FileName != null && _FileDecompress == null)
            {
                this.FileName = _FileName.FullName;
                this.PathFileHUFF = _FileName.FullName + ".huff";
            } 
            if(_FileName == null && _FileDecompress != null)
            {
                this.FileName = _FileDecompress.FullName + ".txt";
                this.PathFileHUFF = _FileDecompress.FullName;
            }
        }



        public List<byte> Comprimir()
        {
            List<int> ListaASCII = new List<int>();
            List<NodoHuffman> ListaProbabilidad = new List<NodoHuffman>();

            ListaASCII = FileTXT_To_ListASCII();

            //Crear Lista Probabilidad
            int NoASCII = -1, Cont = 0;
            double probabilidad = 0;
            for (int x = 0; x < ListaASCII.Count; x++)
            {
                NoASCII = ListaASCII.ElementAt(x);
                if (!Exist(ListaProbabilidad, NoASCII))
                {
                    for (int y = x; y < ListaASCII.Count; y++)
                    {
                        if (ListaASCII.ElementAt(y) == NoASCII)
                            Cont++;
                    }
                    probabilidad = Convert.ToDouble(Decimal.Divide(Cont, ListaASCII.Count));
                    probabilidad = Math.Round(probabilidad, 4);
                    ListaProbabilidad.Add(new NodoHuffman(NoASCII, Cont, probabilidad));
                    Cont = 0;
                }
            }
            List<NodoHuffman> ListaProbOrdenada = ListaProbabilidad.OrderByDescending(nodo => nodo.probability).ToList();
            Dictionary<int, string> Prefijos = ConstruirArbol(ListaProbOrdenada);

            string strTextPre = ConvertTextToPre(ListaASCII, Prefijos);
            List<byte> ListaCompresion = ConvertToByte(strTextPre);
            int frecuenciaMax = FrecuenciaMax(ListaProbabilidad);

            List<byte> FileCompress = ObtenerArchivoComprimido(frecuenciaMax, ListaProbOrdenada, ListaCompresion);

            ObtenerCompressRatio(FileCompress.Count, ListaASCII.Count);
            ObtenerCompressFactor(ListaASCII.Count, FileCompress.Count);

            Console.WriteLine("\n\nCompressRatio: " + this.CompressRatio);
            Console.WriteLine("CompressFactor: " + this.CompressFactor);
            Console.WriteLine("ReductionPer: " + this.ReductionPer + "%");

            return FileCompress;
        }

        private List<int> FileTXT_To_ListASCII()
        {
            List<int> ListaASCII = new List<int>();

            int letter = 0;
            //FileStream stream = new FileStream(fileName, FileMode.Open,
            //FileAccess.Read);
            FileStream stream = fileData.Open(FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);
            while (letter != -1)
            {
                try
                {
                    letter = reader.ReadByte();
                }
                catch
                {
                    letter = reader.Read();
                }

                if (letter != -1)
                {

                    ListaASCII.Add(letter);
                    Console.Write((char)letter);
                }
            }
            reader.Close();
            stream.Close();
            return ListaASCII;
        }
        
        private List<string> ListInt_to_ListStr(List<int> _ListInt)
        {
            List<string> ListaStr = new List<string>();

            if (_ListInt.Count != 0)
            {
                string strbyte = string.Empty;
                for (int i = 0; i < _ListInt.Count; i++)
                {
                    strbyte = ConvertASCIIToStrByte(_ListInt.ElementAt(i));
                    ListaStr.Add(strbyte);
                }
                return ListaStr;
            }
            else
            {
                throw new NullReferenceException();
            }
        }
        
        private string ConvertASCIIToStrByte(int _charASCII)
        {
            byte btindx = 0;
            string strBin = string.Empty;

            btindx = Convert.ToByte(_charASCII);
            strBin = Convert.ToString(btindx, 2);
            strBin = strBin.PadLeft(8, '0');

            return strBin;
        }
        
        private Dictionary<int, string> ConstruirArbol(List<NodoHuffman> _ListaProbabilidad)
        {
            Dictionary<int, string> Prefijos = new Dictionary<int, string>();
            List<NodoHuffman> ListaTemporal = _ListaProbabilidad.ToList();
            NodoHuffman n1, n2, nResult;
            int cont = 1, value = -1;
            double probability = 0;

            while (ListaTemporal.Count != 1)
            {
                n1 = ListaTemporal.Last();
                ListaTemporal.RemoveAt(ListaTemporal.Count - 1);
                n2 = ListaTemporal.Last();
                ListaTemporal.RemoveAt(ListaTemporal.Count - 1);
                value = 300 + cont;
                cont++;
                probability = n1.probability + n2.probability;
                nResult = new NodoHuffman(value, 0, Math.Round(probability, 4));
                nResult.right = n1;
                nResult.left = n2;
                ListaTemporal.Add(nResult);

                List<NodoHuffman> ListaHuffmanOrdenada = ListaTemporal.OrderByDescending(nodo => nodo.probability).ToList();
                ListaTemporal = ListaHuffmanOrdenada;
            }

            string _prefijo = string.Empty;
            for (int x = 0; x < _ListaProbabilidad.Count; x++)
            {
                _prefijo = BuscarArbol(_ListaProbabilidad.ElementAt(x).NoASCII, ListaTemporal.ElementAt(0));
                Prefijos.Add(_ListaProbabilidad.ElementAt(x).NoASCII, _prefijo);
            }

            return Prefijos;
        }
        
        private string BuscarArbol(int NoASCII, NodoHuffman _Raiz)
        {
            string Prefijo = string.Empty;

            RecorridoEnOrden(NoASCII, _Raiz, ref Prefijo);

            return Prefijo;
        }

        private void RecorridoEnOrden(int NoASCII, NodoHuffman _actual, ref string _Prefijo)
        {
            string recorrido = string.Empty;
            RecorridoEnOrdenInterno(NoASCII, _actual, ref recorrido, ref _Prefijo);
        }

        private void RecorridoEnOrdenInterno(int NoASCII, NodoHuffman _actual, ref string _recorrido, ref string _Prefijo)
        {
            if (_actual != null)
            {
                _recorrido = _recorrido + "0";
                RecorridoEnOrdenInterno(NoASCII, _actual.left, ref _recorrido, ref _Prefijo);
                _recorrido = _recorrido.Remove(_recorrido.Length - 1);

                if (_actual.NoASCII == NoASCII)
                {
                    _Prefijo = _recorrido;
                }

                _recorrido = _recorrido + "1";
                RecorridoEnOrdenInterno(NoASCII, _actual.right, ref _recorrido, ref _Prefijo);
                _recorrido = _recorrido.Remove(_recorrido.Length - 1);
            }
        }

        private bool Exist(List<NodoHuffman> _ListaHuffman, int _NoASCII)
        {
            for (int i = 0; i < _ListaHuffman.Count; i++)
            {
                if (_ListaHuffman.ElementAt(i).NoASCII == _NoASCII)
                    return true;
            }

            return false;
        }
        
        private string ConvertTextToPre(List<int> _Text, Dictionary<int, string> _Prefijo)
        {
            string TextoPre = string.Empty;

            for (int x = 0; x < _Text.Count; x++)
            {
                for (int y = 0; y < _Prefijo.Count; y++)
                {
                    if (_Text.ElementAt(x) == _Prefijo.ElementAt(y).Key)
                    {
                        TextoPre = TextoPre + _Prefijo.ElementAt(y).Value;
                    }
                }
            }

            return TextoPre;
        }
        
        private List<byte> ConvertToByte(string _Text)
        {
            List<byte> ListaCompresion = new List<byte>();
            int LongBit = _Text.Length;

            if ((LongBit % 8) != 0)
            {
                int div = LongBit / 8;
                int newLongBit = 8 * (div + 1);
                for (int i = 0; i < (newLongBit - LongBit); i++)
                {
                    _Text = _Text + "0";
                }
                LongBit = newLongBit;
            }

            int cont = 0;
            string _byte = string.Empty;
            for (int x = 0; x < LongBit; x++)
            {
                cont++;
                _byte = _byte + _Text.ElementAt(x);
                if (cont == 8)
                {
                    ListaCompresion.Add(Convert.ToByte(_byte, 2));
                    cont = 0;
                    _byte = string.Empty;
                }
            }

            return ListaCompresion;
        }
        
        private int FrecuenciaMax(List<NodoHuffman> _ListaProbabilidad)
        {
            int frecuenciaMAX = 0;
            int cant = 0;

            for (int i = 0; i < _ListaProbabilidad.Count; i++)
            {
                cant = _ListaProbabilidad.ElementAt(i).frecuency / 255;
                if ((_ListaProbabilidad.ElementAt(i).frecuency % 255) != 0 && (cant + 1) > frecuenciaMAX)
                {
                    frecuenciaMAX = cant + 1;
                }
            }

            return frecuenciaMAX;
        }

        private List<byte> ObtenerArchivoComprimido(int _frecuencyMax, List<NodoHuffman> _ListaProbabilidad, List<byte> _ListaCompresion)
        {
            List<byte> FileCompress = new List<byte>();

            //Escritura de Prefijos
            FileCompress.Add(Convert.ToByte(_frecuencyMax));
            FileCompress.Add(Convert.ToByte(_ListaProbabilidad.Count));

            int temp = 0;
            Stack<int> pilaFrecuencia = new Stack<int>();
            bool ok = false;
            for (int x = 0; x < _ListaProbabilidad.Count; x++)
            {
                FileCompress.Add(Convert.ToByte(_ListaProbabilidad.ElementAt(x).NoASCII));
                for (int y = 0; y < _frecuencyMax; y++)
                {
                    if (ok != true)
                    {
                        temp = temp + 255;

                        if (_ListaProbabilidad.ElementAt(x).frecuency > temp)
                        {
                            pilaFrecuencia.Push(255);
                        }
                        else
                        {

                            pilaFrecuencia.Push(_ListaProbabilidad.ElementAt(x).frecuency - (temp - 255));
                            ok = true;
                        }
                    }
                    else
                    {
                        pilaFrecuencia.Push(0);
                    }
                }
                ok = false;
                temp = 0;
                while (pilaFrecuencia.Count > 0)
                {
                    FileCompress.Add(Convert.ToByte(pilaFrecuencia.Pop()));
                }
            }

            //Escritura de Cadena a comprimir
            for (int z = 0; z < _ListaCompresion.Count; z++)
            {
                FileCompress.Add(Convert.ToByte(_ListaCompresion.ElementAt(z)));
            }

            return FileCompress;
        }

        public void CrearArchivoCompresion()
        {
            if (File.Exists(PathFileHUFF))
            {
                File.Delete(PathFileHUFF);
            }

            //File.Create(PathFileHUFF);
            List<byte> _FileCompress = Comprimir();
            FileStream stream = new FileStream(PathFileHUFF, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(stream);

            for (int x = 0; x < _FileCompress.Count; x++)
            {
                writer.Write(_FileCompress.ElementAt(x));
            }

            writer.Close();
            stream.Close();
        }

        private void ObtenerCompressRatio(int _FileHUFF, int _File)
        {
            this.CompressRatio = Convert.ToDouble(Decimal.Divide(_FileHUFF, _File));
            this.CompressRatio = Math.Round(this.CompressRatio, 2);

            this.ReductionPer = 100 - Convert.ToInt32(this.CompressRatio * 100);
        }

        private void ObtenerCompressFactor(int _File, int _FileHUFF)
        {
            this.CompressFactor = Convert.ToDouble(Decimal.Divide(_File, _FileHUFF));
            this.CompressFactor = Math.Round(this.CompressFactor, 2);
        }



        public List<byte> Descomprimir()
        {
            List<int> ListaASCII = new List<int>();

            ListaASCII = FileHUFF_To_ListASCII();
            List<NodoHuffman> ListaProbabilidad = ObtenerListaProb(ListaASCII);
            Dictionary<int, string> Prefijos = ConstruirArbol(ListaProbabilidad);

            int longListaProb = (ListaASCII.ElementAt(1) * (ListaASCII.ElementAt(0) + 1)) + 2;
            string strCadena = string.Empty;
            for (int x = longListaProb; x < ListaASCII.Count; x++)
            {
                strCadena = strCadena + ConvertASCIIToStrByte(ListaASCII.ElementAt(x));
            }

            List<byte> ListaDescompresion = new List<byte>();
            string cadena = string.Empty;
            int byteASCII = 0;
            int sumFrecuency = LongDescompresion(ListaProbabilidad);
            for (int y = 0; y < strCadena.Length; y++)
            {
                cadena = cadena + strCadena.ElementAt(y);
                byteASCII = EsPrefijo(Prefijos, cadena);
                if (byteASCII != -1)
                {
                    ListaDescompresion.Add(Convert.ToByte(byteASCII));
                    Console.Write((char)byteASCII);
                    cadena = string.Empty;
                    if (sumFrecuency == ListaDescompresion.Count)
                    {
                        break;
                    }
                }
            }

            return ListaDescompresion;
        }

        private List<int> FileHUFF_To_ListASCII()
        {
            List<int> ListaASCII = new List<int>();

            string fileName = PathFileHUFF;
            int letter = 0;
            //FileStream stream = new FileStream(fileName, FileMode.Open,
            //FileAccess.Read);
            FileStream stream = fileDecompress.Open(FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);
            while (letter != -1)
            {
                try
                {
                    letter = reader.ReadByte();
                }
                catch
                {
                    letter = reader.Read();
                }

                if (letter != -1)
                {
                    ListaASCII.Add(letter);
                    Console.Write((char)letter);
                }
            }
            reader.Close();
            stream.Close();
            Console.WriteLine("\n\n");


            return ListaASCII;
        }
        
        private List<NodoHuffman> ObtenerListaProb(List<int> _ListaASCII)
        {
            List<NodoHuffman> ListaProbabilidad = new List<NodoHuffman>();

            int frecuencyMAX = _ListaASCII.ElementAt(0);
            int longListaProb = (_ListaASCII.ElementAt(1) * (frecuencyMAX + 1)) + 2;
            int NoASCII = 0, frecuencia = 0, cantTotal = 0;

            for (int x = 2; x < longListaProb; x++)
            {
                NoASCII = _ListaASCII.ElementAt(x);
                x++;
                for (int y = 0; y < frecuencyMAX; y++)
                {
                    frecuencia = frecuencia + _ListaASCII.ElementAt(x);
                    x++;
                }
                x--;
                cantTotal = cantTotal + frecuencia;
                ListaProbabilidad.Add(new NodoHuffman(NoASCII, frecuencia, 0));
                frecuencia = 0;
            }

            double probabilidad = 0;
            for (int z = 0; z < ListaProbabilidad.Count; z++)
            {
                probabilidad = Convert.ToDouble(Decimal.Divide(ListaProbabilidad.ElementAt(z).frecuency, cantTotal));
                probabilidad = Math.Round(probabilidad, 4);
                ListaProbabilidad.ElementAt(z).probability = probabilidad;
            }

            return ListaProbabilidad;
        }
        
        private int EsPrefijo(Dictionary<int, string> _Prefijos, string _cadena)
        {
            for (int i = 0; i < _Prefijos.Count; i++)
            {
                if (_cadena == _Prefijos.ElementAt(i).Value)
                {
                    return _Prefijos.ElementAt(i).Key;
                }
            }

            return -1;
        }
        
        private int LongDescompresion(List<NodoHuffman> _ListaProbabilidad)
        {
            int sumFrecuencia = 0;
            for (int i = 0; i < _ListaProbabilidad.Count; i++)
            {
                sumFrecuencia = sumFrecuencia + _ListaProbabilidad.ElementAt(i).frecuency;
            }

            return sumFrecuencia;
        }
        
        public void CrearArchivoDescompresion()
        {
            List<byte> ListaDescompresion = Descomprimir();
            try
            {
                if (File.Exists(PathFileHUFF))
                {
                    File.Delete(PathFileHUFF);
                }

                FileStream stream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
                BinaryWriter writer = new BinaryWriter(stream);
                for (int x = 0; x < ListaDescompresion.Count; x++)
                {
                    writer.Write(ListaDescompresion.ElementAt(x));
                }
                writer.Close();
                stream.Close();
            }
            catch
            {

            }
        }
        
        
        private int CompararDouble(double actual, double nuevo)
        {
            return actual.CompareTo(nuevo);
        }

    }
}
