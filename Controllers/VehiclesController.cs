using RentalCars.Data;
using RentalCars.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace RentalCars.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VehiclesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Vehicles
        [HttpGet]
        public IActionResult GetVehicles()
        {
            var vehicles = _context.Vehicles.Where(v => !v.IsDeleted).ToList();
            return Ok(vehicles);
        }

        // GET: api/Vehicles/Filter
        [HttpGet("Filter")]
        public IActionResult FilterVehicles([FromQuery] bool? available, [FromQuery] string? model, [FromQuery] string? color, [FromQuery] int? year)
        {
            var vehicles = _context.Vehicles.Where(v => !v.IsDeleted);

            if (available.HasValue)
                vehicles = vehicles.Where(v => v.Available == available);

            if (!string.IsNullOrEmpty(model))
                vehicles = vehicles.Where(v => v.Model.Contains(model));

            if (!string.IsNullOrEmpty(color))
                vehicles = vehicles.Where(v => v.Color.Contains(color));

            if (year.HasValue)
                vehicles = vehicles.Where(v => v.Year == year);

            return Ok(vehicles.ToList());
        }

        // POST: api/Vehicles
        [HttpPost]
        public IActionResult CreateVehicle([FromBody] Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();
            return Ok(vehicle);
        }

        // PUT: api/Vehicles/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateVehicle(int id, [FromBody] Vehicle updatedVehicle)
        {
            var vehicle = _context.Vehicles.Find(id);
            if (vehicle == null) return NotFound();

            vehicle.Model = updatedVehicle.Model;
            vehicle.Year = updatedVehicle.Year;
            vehicle.Color = updatedVehicle.Color;
            vehicle.Seats = updatedVehicle.Seats;
            vehicle.Price = updatedVehicle.Price;
            vehicle.LicensePlate = updatedVehicle.LicensePlate;
            vehicle.Available = updatedVehicle.Available;

            _context.SaveChanges();
            return Ok(vehicle);
        }
    }
}
