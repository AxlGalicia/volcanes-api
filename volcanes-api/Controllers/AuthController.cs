using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using volcanes_api.Models;
using volcanes_api.Models.DTOs;
using volcanes_api.Utilidades;

namespace volcanes_api.Controllers;

[DynamicRoute("auth")]
[ApiController]
public class AuthController: ControllerBase
{
    private readonly volcanesDBContext _context;
    private readonly IConfiguration _configuration;
    
    public AuthController(volcanesDBContext context,IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult> register(UsuarioDTO request)
    {
        if (request.Username == null)
        {
            BadRequest("El campo de username es obligatorio");
        }else if (request.Password == null)
        {
            BadRequest("El campo de password es obligatorio");
        }

        CreatePasswordHash(request.Password,
                            out byte[] passwordHash,
                            out byte[] passwordSalt );

        var usuario = new Usuario()
        {
            Username = request.Username,
            Password = passwordHash,
            SaltKey = passwordSalt,
            RoleId = 2
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return Ok("Usuario creado");

    }

    [HttpPost("login")]
    public async Task<ActionResult> login(UsuarioDTO request)
    {
        if (request.Username == null)
        {
            BadRequest("El campo de username es obligatorio");
        }else if (request.Password == null)
        {
            BadRequest("El campo de password es obligatorio");
        }

        var usuarioDb = await _context.Usuarios
            .FirstOrDefaultAsync(x => x.Username == request.Username);

        if (usuarioDb == null)
            return NotFound("El username no esta registrado");

        var passwordHash = usuarioDb.Password;
        var passwordSalt = usuarioDb.SaltKey;

        if (!VerifyPasswordHash(request.Password,
                passwordHash,
                passwordSalt))
            return BadRequest("La contraseña es incorrecta");

        var token = JwtGenerate(usuarioDb);
        
        return Ok(token);
    }

    private string JwtGenerate(Usuario usuario)
    {
        List<Claim> claims;
        
        if (usuario.RoleId == 1)
        {
            
            claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,usuario.Username),
                new Claim(ClaimTypes.Role,"Admin")
            };
        }
        else
        {
            claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,usuario.Username),
                new Claim(ClaimTypes.Role,"User")
            };
        }
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JwtKey")));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials:credentials
            );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        
        return jwt;
    }


    private bool VerifyPasswordHash(string password,
                                    byte[] passwordHash,
                                    byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding
                .UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);

        }
        
    }

    private void CreatePasswordHash(string password,
                                    out byte[] passwordHash,
                                    out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
        
    }

}