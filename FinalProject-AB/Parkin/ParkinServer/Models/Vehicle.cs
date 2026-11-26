using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ParkinServer.Models;

[Index(nameof(LicensePlate), IsUnique = true)]
public class Vehicle
{
    [Key]
    public int VehicleId { get; set; }

    [Required]
    public string LicensePlate { get; set; } = null!;

    [Required]
    public string ModelName { get; set; } = null!;

    public string VehicleImagePath { get; set; } = "";

    public virtual ICollection<UserVehicle> UserVehicles { get; set; } = new List<UserVehicle>();
}