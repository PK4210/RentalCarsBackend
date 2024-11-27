using RentalCars.Data;
using RentalCars.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace RentalCars.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(_context.Users.Where(u => !u.IsDeleted).ToList());
        }

        // POST: api/Users/CreateUser
        [HttpPost("CreateUser")]
        public IActionResult CreateUser([FromBody] User user)
        {
            // Crear solo usuarios normales (user_type = 0)
            user.UserType = false; // Usuario común
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok(new { Message = "Usuario creado exitosamente.", User = user });
        }

        // POST: api/Users/Admin/CreateUser
        [Authorize(Roles = "Admin")]
        [HttpPost("Admin/CreateUser")]
        public IActionResult AdminCreateUser([FromQuery] int requestingAdminId, [FromBody] User user)
        {
            var requestingAdmin = _context.Users.FirstOrDefault(u => u.UserId == requestingAdminId);
            if (requestingAdmin == null || requestingAdmin.UserType != true) // Verificar que sea administrador
                return Unauthorized("Solo los administradores pueden crear nuevos usuarios o administradores.");

            // Validar el tipo de usuario a crear
            if (user.UserType != false && user.UserType != true)
                return BadRequest("El campo user_type debe ser 0 (usuario común) o 1 (administrador).");

            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok(new { Message = "Usuario creado exitosamente.", User = user });
        }


        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            user.IsDeleted = true; // Marcar como eliminado (soft delete)
            _context.SaveChanges();
            return NoContent();
        }

        // GET: api/Users/CommonUsers
        [HttpGet("CommonUsers")]
        public IActionResult GetCommonUsers()
        {
            var users = _context.Users
                .Where(u => !u.IsDeleted && u.UserType == false) // Solo usuarios comunes
                .ToList();
            return Ok(users);
        }

        // GET: api/Users/Admins
        [HttpGet("Admins")]
        public IActionResult GetAdmins()
        {
            var admins = _context.Users
                .Where(u => !u.IsDeleted && u.UserType == true) // Solo administradores
                .ToList();
            return Ok(admins);
        }
    }
}
