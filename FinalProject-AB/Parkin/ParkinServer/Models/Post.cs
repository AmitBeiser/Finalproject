using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ParkinServer.Models;

public class Post
{
    [Key]
    public int PostId { get; set; }
    
    [Required]
    public string Caption { get; set; } = null!;
    
    [Required]
    public string ImagePath { get; set; } = null!;
    
    [Required]
    public string LicensePlateId { get; set; } = null!;
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public int UserId { get; set; }
    public int? VehicleId { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
    
    [ForeignKey("VehicleId")]
    public virtual Vehicle? Vehicle { get; set; }
}