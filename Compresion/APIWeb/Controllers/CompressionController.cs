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
    public class CompressionController : ApiController
    {
        // GET: api/Compression
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Compression/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Compression
        [System.Web.Http.HttpPost]
        public async Task<FileResult> Compress()
        {
            byte[] ArrayCompressFile = null;

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new System.Net.Http.MultipartFormDataStreamProvider(root);

            if (Request.Content.IsMimeMultipartContent())
            {
                await Request.Content.ReadAsMultipartAsync(provider);
            }

            FileInfo fileInfoCompress = null;

            foreach (MultipartFileData fileData in provider.FileData)
            {
                fileInfoCompress = new FileInfo(fileData.LocalFileName);
            }

            if (fileInfoCompress != null)
            {
                Compresion.Clases.Huffman huffman = new Compresion.Clases.Huffman(fileInfoCompress, null);
                List<byte> CompressFile = huffman.Comprimir();
                ArrayCompressFile = CompressFile.ToArray();

                huffman.CrearArchivoCompresion();
            }

            return new FileContentResult(ArrayCompressFile, "text/plain")
            {
                FileDownloadName = "Cadena.huff"
            };
        }


        // PUT: api/Compression/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Compression/5
        public void Delete(int id)
        {
        }
    }
}
