using Amazon.S3.Model;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using volcanes_api.Controllers;
using volcanes_api.Models;
using volcanes_api.Models.DTOs;
using volcanes_api.tests.Mocks;

namespace volcanes_api.tests
{
    [TestClass]
    public class VolcanesControllerTest: BasePruebas
    {
        [TestMethod]
        public async Task obtenerTodosLosVolcanes()
        {
            //Preparacion
            var nombreDB = Guid.NewGuid().ToString();
            var contextoConfig = ConstruirContexto(nombreDB);
            contextoConfig.Volcans.Add(new Volcan() 
            { 
                Id=1,
                Nombre="volcan-1",
                Descripcion="Descripcion del volcan",
                Ubicacion="Guatemala",
                Altura=3500,
                Ecosistema="Ambiente seco",
               Imagen="imagen-1"
            });

            contextoConfig.Volcans.Add(new Volcan()
            {
                Id = 2,
                Nombre = "volcan-2",
                Descripcion = "Descripcion del volcan",
                Ubicacion = "Guatemala",
                Altura = 3500,
                Ecosistema = "Ambiente seco",
                Imagen = "imagen-2"
            });

            await contextoConfig.SaveChangesAsync();

            //Prueba

            var contextoPrueba = ConstruirContexto(nombreDB);
            var logger = new MockILogger<VolcanesController>();
            var spaceService = new MockSpacesDigitalOceanService();
            var controller = new VolcanesController(contextoPrueba,
                                                    logger,
                                                    spaceService);
            var response = await controller.get();

            //Verificacion
            Assert.AreEqual(2,response.Count());
        }

        [TestMethod]
        public async Task obtenerUnSoloVolcan()
        { 
            //Preparacion
            var nombreDB = Guid.NewGuid().ToString();
            var contextoConfig = ConstruirContexto(nombreDB);
            await contextoConfig.Volcans.AddAsync(new Volcan() 
            {
                Id = 1,
                Nombre = "volcan-1",
                Descripcion = "Descripcion del volcan",
                Altura = 4000.5,
                Ecosistema = "Ambien seco",
                Ubicacion = "Guatemala",
                Imagen = "imagen-1"
            });
            await contextoConfig.SaveChangesAsync();

            //Prueba
            var contextoPrueba = ConstruirContexto(nombreDB);
            var logger = new MockILogger<VolcanesController>();
            var spaceService = new MockSpacesDigitalOceanService();
            var controller = new VolcanesController(contextoPrueba,
                                                    logger,
                                                    spaceService);
            var response = await controller.getId(1);

            //Verificacion
            Assert.AreEqual(1,response.Value.Id);
        }

        [TestMethod]
        public async Task obtenerUnSoloVolcanFallido()
        {
            //Preparacion
            var nombreDB = Guid.NewGuid().ToString();
            var contextoPrueba = ConstruirContexto(nombreDB);
            var logger = new MockILogger<VolcanesController>();

            //Prueba
            var spaceService = new MockSpacesDigitalOceanService();
            var controller = new VolcanesController(contextoPrueba,
                                                    logger,
                                                    spaceService);
            var respuesta = await controller.getId(1);
            var resultado = respuesta.Result as StatusCodeResult;
            Assert.AreEqual(404,resultado.StatusCode);
        }

        [TestMethod]
        public async Task postearUnVolcanSinImagen()
        {
            //Preparacion
            var nombreDB = Guid.NewGuid().ToString();

            //Prueba
            var contextoPrueba = ConstruirContexto(nombreDB);
            var logger = new MockILogger<VolcanesController>();
            var spaceService = new MockSpacesDigitalOceanService();
            var controller = new VolcanesController(contextoPrueba,
                                                    logger,
                                                    spaceService);

            var volcan = new VolcanCreacionDTO()
            {
                Nombre = "volcan prueba",
                Descripcion = "Descripcion del volcan.",
                Altura = 4000,
                Ubicacion = "Guatemala",
                Ecosistema = "Bosque",
                Imagen = null
            };
            var response = await controller.post(volcan);

            //Verificacion

            var respuesta = response as StatusCodeResult;
            Assert.AreEqual(204,respuesta.StatusCode);


        }

        [TestMethod]
        public async Task postearUnVolcanConImagen()
        {
            //Preparacion
            var nombreDB = Guid.NewGuid().ToString();
            var archivoPrueba = obtenerArchivoDePrueba();
            var volcan = new VolcanCreacionDTO()
            {
                
                Nombre = "volcan",
                Descripcion = "descripcion",
                Altura = 5000,
                Ecosistema = "bosque",
                Ubicacion = "Guatemala",
                Imagen = archivoPrueba

            };

            //Prueba
            var contextoPrueba = ConstruirContexto(nombreDB);
            var logger = new MockILogger<VolcanesController>();
            var spaceService = new MockSpacesDigitalOceanService();
            var controller = new VolcanesController(contextoPrueba,logger,spaceService);

            //Verificacion
            var response = controller.post(volcan);

            var respuesta = response.Result as StatusCodeResult;
            Assert.AreEqual(204,respuesta.StatusCode);

        }

        [TestMethod]
        public async Task actualizarUnVolcan()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contextoConfig = ConstruirContexto(nombreDB);
            var volcanDB = new Volcan()
            {
                Nombre = "volcan",
                Descripcion = "descripcion volcan",
                Ecosistema = "Bosque",
                Altura = 5000,
                Ubicacion = "Guatemala",
                Imagen = "Imagen.png"
            };

            contextoConfig.Volcans.Add(volcanDB);
            await contextoConfig.SaveChangesAsync();

            var volcanActualizado = new Volcan()
            {
                Id = 1,
                Nombre = "volcan (ACTUALIZADO)",
                Descripcion = "descripcion volcan",
                Ecosistema = "Bosque",
                Altura = 5000,
                Ubicacion = "Guatemala",
                Imagen = "Imagen.png"
            };

            var contextoPrueba = ConstruirContexto(nombreDB);
            var logger = new MockILogger<VolcanesController>();
            var spaceService = new MockSpacesDigitalOceanService();
            var id = 1;

            var controller = new VolcanesController(contextoPrueba,
                                                    logger,
                                                    spaceService);
            var response = await controller.put(volcanActualizado, id);

            var respuesta = response as StatusCodeResult;

            Assert.AreEqual(204,respuesta.StatusCode);

            var responseBadRequest = await controller.put(volcanActualizado, 10);

            var respuestaBadRequest = responseBadRequest as BadRequestObjectResult;

            Assert.AreEqual(400,respuestaBadRequest.StatusCode);
        }

        [TestMethod]
        public async Task eliminarUnVolcan()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contextoConfig = ConstruirContexto(nombreDB);
            var volcanDB = new Volcan()
            {
                Nombre = "volcan",
                Descripcion = "descripcion volcan",
                Ecosistema = "Bosque",
                Altura = 5000,
                Ubicacion = "Guatemala",
                Imagen = "Imagen.png"
            };

            contextoConfig.Volcans.Add(volcanDB);
            await contextoConfig.SaveChangesAsync();

            var contextoPrueba = ConstruirContexto(nombreDB);
            var logger = new MockILogger<VolcanesController>();
            var spaceService = new MockSpacesDigitalOceanService();
            var id = 1;

            var controller = new VolcanesController(contextoPrueba,
                                                    logger,
                                                    spaceService);

            var response = await controller.delete(id);

            var respuesta = response as StatusCodeResult;

            Assert.AreEqual(204, respuesta.StatusCode);

            var responseNotFound = await controller.delete(10);

            var respuestaNotFound = responseNotFound as StatusCodeResult;

            Assert.AreEqual(404, respuestaNotFound.StatusCode);
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