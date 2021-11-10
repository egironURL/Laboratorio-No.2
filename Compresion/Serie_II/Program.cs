using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Compresion.Clases;

namespace Serie_II
{
    class Program
    {
        static void Main(string[] args)
        {
            Huffman Compresion = new Huffman(new FileInfo(@"C:\Users\IT\Documents\EG\Cadena.txt"), new FileInfo(@"C:\Users\IT\Documents\EG\Cadena.huff"));
            Compresion.Comprimir();
            //Compresion.CrearArchivoDescompresion();
            Console.ReadLine();
        }

        
    }
}
