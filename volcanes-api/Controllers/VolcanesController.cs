using Amazon.Runtime.Internal;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using volcanes_api.Models;
using volcanes_api.Models.DTOs;

namespace volcanes_api.Controllers
{
    [ApiController]
    [Route("api/volcanes")]
    public class VolcanesController : ControllerBase
    {
        private readonly volcanesDBContext _context;
        private readonly ILogger<volcanesDBContext> _logger;

        public VolcanesController(volcanesDBContext context,
                                    ILogger<volcanesDBContext> logger)
        {
            _context = context;
            _logger = logger;
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
            var volcan = await _context.Volcans.FirstOrDefaultAsync(x => x.Id == id);
            if (volcan == null)
                return NotFound();
            return volcan;
        }

        private void InformationMessage(string message)
        {
            _logger.LogInformation(message);
        }
    }
}   
