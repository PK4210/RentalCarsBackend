using RentalCars.Data;
using RentalCars.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace RentalCars.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Solo accesible para administradores
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        public class DateRange
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        // Informe de vehículos alquilados entre dos fechas
        [HttpPost("rented-vehicles")]
        public IActionResult GetRentedVehicles([FromBody] DateRange dateRange)
        {
            if (dateRange == null || dateRange.StartDate >= dateRange.EndDate)
            {
                return BadRequest("Rango de fechas inválido.");
            }

            var rentals = _context.Rentals
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .Where(r => !r.IsDeleted &&
                            r.StartDate >= dateRange.StartDate &&
                            r.EndDate <= dateRange.EndDate)
                .Select(r => new
                {
                    r.RentalId,
                    User = new { r.User.UserId, r.User.Username, r.User.Email },
                    Vehicle = new { r.Vehicle.VehicleId, r.Vehicle.Model, r.Vehicle.LicensePlate },
                    StartDate = r.StartDate.ToString("yyyy-MM-ddTHH:mm:ss"), // Formato ISO
                    EndDate = r.EndDate.ToString("yyyy-MM-ddTHH:mm:ss"),     // Formato ISO
                    r.TotalPrice,
                    r.TotalDays
                })
                .ToList();

            return Ok(rentals);
        }

        // Informe de vehículos con préstamos vencidos
        [HttpGet("overdue-rentals")]
        public IActionResult GetOverdueRentals()
        {
            var today = DateTime.Now;

            var overdueRentals = _context.Rentals
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .Where(r => r.EndDate < today && !r.IsDeleted)
                .Select(r => new
                {
                    r.RentalId,
                    User = new { r.User.UserId, r.User.Username, r.User.Email },
                    Vehicle = new { r.Vehicle.VehicleId, r.Vehicle.Model, r.Vehicle.LicensePlate },
                    StartDate = r.StartDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    EndDate = r.EndDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    r.TotalPrice,
                    OverdueDays = (today - r.EndDate).Days
                })
                .ToList();

            return Ok(overdueRentals);
        }
    }
}
