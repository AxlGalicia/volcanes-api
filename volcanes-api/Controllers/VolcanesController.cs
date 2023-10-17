using Amazon.Runtime.Internal;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using volcanes_api.Interfaces;
using volcanes_api.Models;
using volcanes_api.Models.DTOs;

namespace volcanes_api.Controllers
{
    [ApiController]
    [Route("api/volcanes")]
    public class VolcanesController : ControllerBase
    {
        private readonly volcanesDBContext _context;
        private readonly ILogger<VolcanesController> _logger;
        private readonly ISpacesDigitalOceanService _spaceService;

        public VolcanesController(volcanesDBContext context,
                                  ILogger<VolcanesController> logger,
                                  ISpacesDigitalOceanService spaceService)
        {
            _context = context;
            _logger = logger;
            _spaceService = spaceService;
        }

        [HttpGet]
        public async Task<List<Volcan>> get()
        {
            InformationMessage("Se ejecuto solicitud GET");

            var volcanes = await _context.Volcans.ToListAsync();
            return volcanes;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Volcan>> getId(int id)
        {
            InformationMessage("Se ejecuto solicitud GET by Id");

            var volcan = await _context.Volcans.FirstOrDefaultAsync(x => x.Id == id);
            if (volcan == null)
                return NotFound();
            return volcan;
        }

        [HttpPost]
        public async Task<ActionResult> post([FromForm]VolcanDTO volcan)
        {
            InformationMessage("Se ejecuto solicitud Post");

            var volcanDB = new Volcan()
            {
                Nombre = volcan.Nombre,
                Descripcion = volcan.Descripcion,
                Altura = volcan.Altura,
                Ecosistema = volcan.Ecosistema,
                Ubicacion = volcan.Ubicacion
            };

            if (volcan.Imagen != null)
            {
                var response = await _spaceService.UploadFileAsync(volcan.Imagen);
                if (response)
                {
                    InformationMessage("Se guardo correctamente la imagen.");
                    volcanDB.Imagen = volcan.Imagen.FileName;
                    //return Ok();
                }
                else
                {
                    WarningMessage("Hubo un problema al subir la imagen.");
                    volcanDB.Imagen = "";
                    //return Conflict();
                }
            }
            else 
            {
                InformationMessage("No se envio una imagen para el registro");
                volcanDB.Imagen = "";
            }


            _context.Volcans.Add(volcanDB);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> put([FromBody]Volcan volcan,int id)
        {
            InformationMessage("Se ejecuto solicitud PUT");

            if (volcan.Id != id)
                return BadRequest("Los IDs no coinciden");

            var existe = await _context.Volcans.AnyAsync(x => x.Id == volcan.Id);

            if (!existe)
                return NotFound("El objeto no se encontro");

            _context.Entry(volcan).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private void InformationMessage(string message)
        {
            _logger.LogInformation(message);
        }

        private void WarningMessage(string message)
        { 
            _logger.LogWarning(message);
        }
    }
}   
