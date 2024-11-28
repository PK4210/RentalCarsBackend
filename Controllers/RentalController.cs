using RentalCars.Data; 
using RentalCars.Models;    // Para acceder a User, Vehicle, Rental
using Microsoft.AspNetCore.Mvc; // Para controladores
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace RentalCars.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Solo accesible para administradores
    public class RentalsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RentalsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Rentals
        [HttpGet]
        public IActionResult GetRentals()
        {
            var rentals = _context.Rentals
                .Where(r => !r.IsDeleted)
                .Include(r => r.User) // Incluir información del usuario
                .Include(r => r.Vehicle) // Incluir información del vehículo
                .Select(r => new
                {
                    r.RentalId,
                    User = new { r.User.UserId, r.User.Username, r.User.Email },
                    Vehicle = new { r.Vehicle.VehicleId, r.Vehicle.Model, r.Vehicle.LicensePlate },
                    r.StartDate,
                    r.EndDate,
                    r.TotalPrice,
                    r.TotalDays,
                    r.IsDeleted,
                    r.CreatedAt
                })
                .ToList();

            return Ok(rentals);
        }




        // POST: api/Rentals
        [HttpPost]
        public async Task<IActionResult> CreateRental([FromBody] Rental rental)
        {
            // Validar si el vehículo existe y está disponible
            var vehicle = await _context.Vehicles.FindAsync(rental.VehicleId);
            if (vehicle == null || !vehicle.Available)
            {
                return BadRequest(new { message = "El vehículo no existe o no está disponible" });
            }

            // Calcular el totalPrice
            rental.TotalPrice = vehicle.Price * rental.TotalDays;

            // Calcular el endDate basado en startDate y totalDays
            rental.EndDate = rental.StartDate.AddDays(rental.TotalDays);

            // Marcar el vehículo como no disponible
            vehicle.Available = false;

            // Guardar el nuevo alquiler
            _context.Rentals.Add(rental);

            // Actualizar el estado del vehículo
            _context.Vehicles.Update(vehicle);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRentals), new { id = rental.RentalId }, rental);
        }


        // PUT: api/Rentals/Return/{id}
        [HttpPut("Return/{id}")]
        public IActionResult ReturnRental(int id)
        {
            var rental = _context.Rentals.Find(id);
            if (rental == null) return NotFound();

            var vehicle = _context.Vehicles.Find(rental.VehicleId);
            if (vehicle != null)
                vehicle.Available = true;

            rental.IsDeleted = true;
            _context.SaveChanges();

            return NoContent();
        }

        // GET: api/Rentals/Report
        [HttpGet("Report")]
        public IActionResult GetRentedVehicles([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var rentals = _context.Rentals
                .Where(r => r.StartDate >= startDate && r.EndDate <= endDate && !r.IsDeleted)
                .ToList();

            return Ok(rentals);
        }
    }

}
