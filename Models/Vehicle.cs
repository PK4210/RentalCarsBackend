namespace RentalCars.Models
{
    public class Vehicle
    {
        public int VehicleId { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public int Seats { get; set; }
        public decimal Price { get; set; }
        public string LicensePlate { get; set; }
        public string ImageUrl { get; set; }
        public bool Available { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
