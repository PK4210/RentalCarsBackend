using RentalCars.Data;
using RentalCars.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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

        // Informe de vehículos alquilados entre dos fechas
        [HttpPost("rentedvehicles")]
        public IActionResult GetRentedVehicles([FromBody] DateRange dateRange)
        {
            var rentals = _context.Rentals
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

        public class DateRange
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
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
