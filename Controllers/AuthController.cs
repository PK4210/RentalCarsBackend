using Microsoft.AspNetCore.Mvc;
using RentalCars.Data;
using RentalCars.Models; // Asegúrate de incluir este namespace

namespace RentalCars.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
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

            // Retornar datos del usuario
            return Ok(new
            {
                Message = "Inicio de sesión exitoso",
                User = new
                {
                    user.UserId,
                    user.Username,
                    user.Email,
                    user.UserType // true: Admin, false: Usuario normal
                }
            });
        }
    }
}
