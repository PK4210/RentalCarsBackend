using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RentalCars.Data;
using RentalCars.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RentalCars.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/Auth/Login
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            // Buscar usuario por nombre de usuario
            var user = _context.Users.FirstOrDefault(u => u.Username == login.Username);

            // Validar contraseña
            if (user == null || user.Password != login.Password || user.IsDeleted)
            {
                return Unauthorized("Usuario o contraseña incorrectos, o el usuario está eliminado.");
            }

            // Generar token JWT
            var token = GenerateJwtToken(user);

            // Retornar token y datos del usuario
            return Ok(new
            {
                Message = "Inicio de sesión exitoso",
                Token = token,
                User = new
                {
                    user.UserId,
                    user.Username,
                    user.Email,
                    user.UserType // true: Admin, false: Usuario normal
                }
            });
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.UserType ? "Admin" : "User") // Asignar rol según el tipo de usuario
    };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
