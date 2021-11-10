using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compresion.Clases
{
    public class Compresion
    {
        public string FileName { get; set; }

        public string PathFileHUFF { get; set; }

        public double CompressRatio { get; set; }

        public double CompressFactor { get; set; }

        public int ReductionPer { get; set; }

        public void ObtenerDatos(Huffman huffman)
        {
            this.FileName = huffman.FileName;
            this.PathFileHUFF = huffman.PathFileHUFF;
            this.CompressRatio = huffman.CompressRatio;
            this.CompressFactor = huffman.CompressFactor;
            this.ReductionPer = huffman.ReductionPer;
        }
    }
}
