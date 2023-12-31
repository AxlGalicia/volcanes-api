﻿using Amazon.Runtime.Internal;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using volcanes_api.Interfaces;
using volcanes_api.Models;
using volcanes_api.Models.DTOs;
using volcanes_api.Utilidades;

namespace volcanes_api.Controllers
{
    [ApiController]
    [DynamicRoute("volcanes_usuario")]
    public class VolcanesUsuarioController : ControllerBase
    {
        private readonly volcanesDBContext _context;
        private readonly ILogger<VolcanesUsuarioController> _logger;
        private readonly ISpacesDigitalOceanService _spaceService;
        private readonly IHeaderService _headerService;

        public VolcanesUsuarioController(volcanesDBContext context,
                                  ILogger<VolcanesUsuarioController> logger,
                                  ISpacesDigitalOceanService spaceService,
                                  IHeaderService headerService)
        {
            _context = context;
            _logger = logger;
            _spaceService = spaceService;
            _headerService = headerService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<List<VolcanUsuario>>> get([FromQuery] PaginacionDTO paginacionDto)
        {
            InformationMessage("Se ejecuto solicitud GET");

            var queryable = _context.VolcanUsuarios.AsQueryable();

            //await HttpContext.InsertarParametros(queryable);
            await _headerService.InsertarParametros(HttpContext, queryable);
            
            var volcanes = await queryable
                                            .paginar(paginacionDto)
                                            .ToListAsync();

            if (volcanes.Count < 1)
                return Ok("No hay registros, pero puedes crear tus propios registros a este mismo endpoint");
            
            return volcanes;
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<VolcanUsuario>> getId(int id)
        {
            InformationMessage("Se ejecuto solicitud GET by Id");

            var volcan = await _context.VolcanUsuarios.FirstOrDefaultAsync(x => x.Id == id);
            if (volcan == null)
                return NotFound("No se encontro el objeto");

            return volcan;
        }

        [HttpGet("imagen/{id:int}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> getImage(int id)
        {
            InformationMessage("Se ejecuto solicitud GET by id Imagen");

            var volcan = await _context.VolcanUsuarios.FindAsync(id);
            if (volcan == null)
                return NotFound("El registro del volcan no existe");

            var response = await _spaceService.DownloadFileAsync(volcan.Imagen);

            if (response == null)
                return NotFound("No se encontro registro de la imagen guardada");

            return File(response.contenido,response.tipoContenido);
            
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> post([FromForm] VolcanCreacionDTO volcan)
        {
            InformationMessage("Se ejecuto solicitud Post");

            var volcanDB = new VolcanUsuario()
            {
                Nombre = volcan.Nombre,
                Descripcion = volcan.Descripcion,
                Altura = volcan.Altura,
                Ecosistema = volcan.Ecosistema,
                Ubicacion = volcan.Ubicacion
            };

            if (volcan.Imagen != null)
            {
                if (!validateFile(volcan.Imagen))
                    return BadRequest("Tiene que ser una imagen con alguna de las siguientes extensiones(.png, .jpg, .jpeg, .gif)");
                
                var response = await _spaceService.UploadFileAsync(volcan.Imagen);
                if (response.responseStatus)
                {
                    InformationMessage("Se guardo correctamente la imagen.");
                    volcanDB.Imagen = response.newName;
                }
                else
                {
                    WarningMessage("Hubo un problema al subir la imagen.");
                    volcanDB.Imagen = "";
                }
            }
            else
            {
                InformationMessage("No se envio una imagen para el registro");
                volcanDB.Imagen = "";
            }


            _context.VolcanUsuarios.Add(volcanDB);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> put([FromBody] VolcanActualizarDTO volcanActualizarDto, int id)
        {
            InformationMessage("Se ejecuto solicitud PUT");

            if (volcanActualizarDto.Id != id)
                return BadRequest("Los IDs no coinciden");

            var volcanDB = await _context.VolcanUsuarios.FindAsync(volcanActualizarDto.Id);

            if (volcanDB == null)
                return NotFound("El objeto no se encontro");

            volcanDB.Id = volcanActualizarDto.Id;
            volcanDB.Nombre = volcanActualizarDto.Nombre;
            volcanDB.Descripcion = volcanActualizarDto.Descripcion;
            volcanDB.Altura = volcanActualizarDto.Altura;
            volcanDB.Ubicacion = volcanActualizarDto.Ubicacion;
            volcanDB.Ecosistema = volcanActualizarDto.Ecosistema;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> delete(int id)
        {
            var volcan = await _context.VolcanUsuarios.FindAsync(id);

            if (volcan == null)
                return NotFound("No se encontro registro del objeto");

            await _spaceService.DeleteFileAsync(volcan.Imagen);

            _context.VolcanUsuarios.Remove(volcan);
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

        private bool validateFile(IFormFile file)
        {
            var extensionesPermitidas = new string[]
            {
                ".png",
                ".jpg",
                ".jpeg",
                ".gif"
            };

            var extensionFile = Path.GetExtension(file.FileName).ToLower();

            if (!extensionesPermitidas.Contains(extensionFile))
                return false;

            return true;
        }
    }
}   
