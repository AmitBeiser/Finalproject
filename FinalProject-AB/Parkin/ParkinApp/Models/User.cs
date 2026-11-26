namespace ParkinApp.Models;

public class User
{
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ProfileImagePath { get; set; } = "";
    
    public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}