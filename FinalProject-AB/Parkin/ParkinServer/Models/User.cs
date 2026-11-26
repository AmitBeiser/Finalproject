using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ParkinServer.Models;

[Index(nameof(Email), IsUnique = true)]
[Index(nameof(UserName), IsUnique = true)]
public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    public string FullName { get; set; } = null!;

    [Required]
    public string UserName { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    public string ProfileImagePath { get; set; } = "";

    public virtual ICollection<UserVehicle> UserVehicles { get; set; } = new List<UserVehicle>();
}