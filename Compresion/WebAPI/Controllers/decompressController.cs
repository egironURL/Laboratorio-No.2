using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Compresion;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class decompressController : ControllerBase
    {
        // POST  /api/decompress
        [HttpPost("{name}")]
        public async Task<FileResult> Download(string name)
        {
            var files = Request.Form.Files;
            var fileName = name + ".txt";
            FileInfo fileInfoDecompress = null;

            if (files.Count == 1)
            {
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var filePath = Path.GetTempFileName();

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        fileInfoDecompress = new FileInfo(filePath);
                    }
                    if (fileInfoDecompress != null)
                    {
                        Compresion.Clases.Huffman compresion = new Compresion.Clases.Huffman(null, fileInfoDecompress);
                        List<byte> ArrayDecompressFile = compresion.Descomprimir();
                        var ArrayBytesDecompress = ArrayDecompressFile.ToArray();

                        return File(ArrayBytesDecompress, "application/octet-stream", fileName);
                    }
                }
            }

            return null;
        }
    }
}