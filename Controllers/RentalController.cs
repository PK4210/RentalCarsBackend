using RentalCars.Data; 
using RentalCars.Models;    // Para acceder a User, Vehicle, Rental
using Microsoft.AspNetCore.Mvc; // Para controladores
using System.Linq;

namespace RentalCars.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var rentals = _context.Rentals.Where(r => !r.IsDeleted).ToList();
            return Ok(rentals);
        }

        // POST: api/Rentals
        [HttpPost]
        public IActionResult CreateRental([FromBody] Rental rental)
        {
            var vehicle = _context.Vehicles.Find(rental.VehicleId);
            if (vehicle == null || !vehicle.Available)
                return BadRequest("El vehículo no está disponible.");

            rental.TotalDays = (rental.EndDate - rental.StartDate).Days;
            rental.TotalPrice = rental.TotalDays * vehicle.Price;

            vehicle.Available = false; // Marcar como no disponible
            _context.Rentals.Add(rental);
            _context.SaveChanges();

            return Ok(rental);
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
