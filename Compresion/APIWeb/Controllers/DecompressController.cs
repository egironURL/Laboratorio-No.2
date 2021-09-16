using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace APIWeb.Controllers
{
    public class DecompressController : ApiController
    {
        // GET: api/Decompress
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Decompress/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Decompress
        [System.Web.Http.HttpPost]
        public async Task<FileResult> Decompress()
        {
            byte[] ArrayDecompressFile = null;

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new System.Net.Http.MultipartFormDataStreamProvider(root);

            if (Request.Content.IsMimeMultipartContent())
            {
                await Request.Content.ReadAsMultipartAsync(provider);
            }

            FileInfo fileInfoDecompress = null;

            foreach (MultipartFileData fileData in provider.FileData)
            {
                fileInfoDecompress = new FileInfo(fileData.LocalFileName);
            }

            if (fileInfoDecompress != null)
            {
                Compresion.Clases.Huffman huffman = new Compresion.Clases.Huffman(null, fileInfoDecompress);
                List<byte> DecompressFile = huffman.Descomprimir();
                ArrayDecompressFile = DecompressFile.ToArray();

                huffman.CrearArchivoDescompresion();
            }

            return new FileContentResult(ArrayDecompressFile, "text/plain")
            {
                FileDownloadName = "Cadena.txt"
            };
        }

        // PUT: api/Decompress/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Decompress/5
        public void Delete(int id)
        {
        }
    }
}
