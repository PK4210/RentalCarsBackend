using RentalCars.Data;
using RentalCars.Models;
using Microsoft.AspNetCore.Mvc;

namespace RentalCars.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        // Informe de vehículos alquilados entre dos fechas
        [HttpGet("RentedVehicles")]
        public IActionResult GetRentedVehicles([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var rentals = _context.Rentals
                .Where(r => r.StartDate >= startDate && r.EndDate <= endDate && !r.IsDeleted)
                .Select(r => new
                {
                    RentalId = r.RentalId,
                    Vehicle = _context.Vehicles.FirstOrDefault(v => v.VehicleId == r.VehicleId),
                    User = _context.Users.FirstOrDefault(u => u.UserId == r.UserId),
                    r.StartDate,
                    r.EndDate,
                    r.TotalPrice,
                    r.TotalDays
                })
                .ToList();

            return Ok(rentals);
        }

        // Informe de clientes con préstamos vencidos
        [HttpGet("OverdueRentals")]
        public IActionResult GetOverdueRentals()
        {
            var today = DateTime.Now;
            var overdueRentals = _context.Rentals
                .Where(r => r.EndDate < today && !r.IsDeleted)
                .Select(r => new
                {
                    RentalId = r.RentalId,
                    User = _context.Users.FirstOrDefault(u => u.UserId == r.UserId),
                    Vehicle = _context.Vehicles.FirstOrDefault(v => v.VehicleId == r.VehicleId),
                    r.StartDate,
                    r.EndDate,
                    r.TotalPrice,
                    OverdueDays = (today - r.EndDate).Days
                })
                .ToList();

            return Ok(overdueRentals);
        }
    }
}
