using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ParkinServer.Models;

[PrimaryKey(nameof(UserId), nameof(VehicleId))]
public class UserVehicle
{
    public int UserId { get; set; }
    public int VehicleId { get; set; }

    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    [ForeignKey("VehicleId")]
    public virtual Vehicle? Vehicle { get; set; }
}