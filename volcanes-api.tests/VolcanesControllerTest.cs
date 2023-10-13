using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using volcanes_api.Controllers;
using volcanes_api.Models;
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
            var controller = new VolcanesController(contextoPrueba,logger);
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
            var controller = new VolcanesController(contextoPrueba, logger);
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
            var controller = new VolcanesController(contextoPrueba,logger);
            var respuesta = await controller.getId(1);
            var resultado = respuesta.Result as StatusCodeResult;
            Assert.AreEqual(404,resultado.StatusCode);
        }
    }
}