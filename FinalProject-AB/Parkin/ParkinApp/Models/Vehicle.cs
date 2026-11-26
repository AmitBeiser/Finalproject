namespace ParkinApp.Models;

public class Vehicle
{
    public int VehicleId { get; set; }
    public string LicensePlate { get; set; } = null!;
    public string ModelName { get; set; } = null!;
    public string VehicleImagePath { get; set; } = "";
    public int UserId { get; set; }
}