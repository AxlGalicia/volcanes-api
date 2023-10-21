using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using volcanes_api.Interfaces;
using volcanes_api.Models.DTOs;

namespace volcanes_api.tests.Mocks
{
    internal class MockSpacesDigitalOceanService : ISpacesDigitalOceanService
    {
        public Task<bool> DeleteFileAsync(string fileName, string versionId = "")
        {
            return Task.FromResult(true);
        }

        public Task<ArchivoDescargadoDTO> DownloadFileAsync(string file)
        {
            //var archivo = obtenerArchivoDePrueba();
            byte[] contenido = System.Text.Encoding.UTF8.GetBytes("Este es un archivo en memoria");
            var archivo = new ArchivoDescargadoDTO();
            archivo.contenido = contenido;
            archivo.tipoContenido = "image/png";
            return Task.FromResult(archivo);
        }

        public Task<bool> UploadFileAsync(IFormFile file)
        {
            return Task.FromResult(true);
        }
        
        private IFormFile obtenerArchivoDePrueba()
        {
            byte[] contenido = System.Text.Encoding.UTF8.GetBytes("Este es un archivo en memoria");
            byte[] archivoEnBytes;
            FormFile respuesta;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(contenido, 0, contenido.Length);
                //archivoEnBytes = memoryStream.ToArray();
                respuesta = new FormFile(memoryStream,0,memoryStream.Length,"application/octet-stream","archivo.txt");
            }

            return respuesta;

        }
    }
}
