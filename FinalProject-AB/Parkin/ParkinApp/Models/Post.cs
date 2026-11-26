namespace ParkinApp.Models;

public class Post
{
    public int PostId { get; set; }
    
    public string Caption { get; set; } = null!;
    
    public string ImagePath { get; set; } = null!;
    
    public string LicensePlateId { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public int UserId { get; set; }
    public int? VehicleId { get; set; }
    
    public virtual User? User { get; set; }
    
    public virtual Vehicle? Vehicle { get; set; }   
}