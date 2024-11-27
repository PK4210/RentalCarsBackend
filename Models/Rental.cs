namespace RentalCars.Models
{
    public class Rental
    {
        public int RentalId { get; set; }
        public int UserId { get; set; }
        public int VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalDays { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }

        // Relaciones
        public User User { get; set; }
        public Vehicle Vehicle { get; set; }
    }

}
