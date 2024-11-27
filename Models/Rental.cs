namespace RentalCars.Models
{
    public class Rental
    {
        public int RentalId { get; set; }
        public int UserId { get; set; } // Clave foránea de Usuario
        public int VehicleId { get; set; } // Clave foránea de Vehículo
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalDays { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }

        // Propiedades de navegación opcionales
        public User? User { get; set; }
        public Vehicle? Vehicle { get; set; }
    }


}
