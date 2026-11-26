using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkinServer.DTO;
using ParkinServer.Models;

namespace ParkinServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ParkinDbContext _context;

    public UsersController(ParkinDbContext context)
    {
        _context = context;
    }

    // 1. בדיקה אם רכב כבר קיים לפי מספר רישוי (עבור הפיצ'ר של שיתוף רכב)
    [HttpGet("CheckVehicle")]
    public async Task<IActionResult> CheckVehicle([FromQuery] string licensePlate)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.UserVehicles)
            .ThenInclude(uv => uv.User)
            .FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);

        if (vehicle == null)
            return NotFound(); // הרכב לא קיים - אפשר ליצור חדש

        // מחזירים את פרטי הרכב הקיים ואת שמות המשתמש של השותפים הנוכחיים
        var ownerUsernames = vehicle.UserVehicles
            .Where(uv => uv.User != null)
            .Select(uv => "@" + uv.User!.UserName)
            .ToList();

        return Ok(new
        {
            vehicle.VehicleId,
            vehicle.LicensePlate,
            vehicle.ModelName,
            vehicle.VehicleImagePath,
            Owners = string.Join(", ", ownerUsernames)
        });
    }

    // 2. הרשמת משתמש חדש + יצירה או שיוך של הרכבים שלו
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email is already registered.");

        if (await _context.Users.AnyAsync(u => u.UserName == request.UserName))
            return BadRequest("Username is already taken.");

        var newUser = new User
        {
            FullName = request.FullName,
            UserName = request.UserName,
            Email = request.Email,
            Password = request.Password,
            ProfileImagePath = request.ProfileImagePath
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync(); // שומר כדי לקבל UserId חדש

        // מעבר על הרכבים שהמשתמש הזין בהרשמה
        foreach (var v in request.Vehicles)
        {
            var existingVehicle = await _context.Vehicles
                .FirstOrDefaultAsync(dbV => dbV.LicensePlate == v.LicensePlate);

            if (existingVehicle != null)
            {
                // הרכב כבר קיים -> מוסיפים רק קישור בטבלת הגישור!
                _context.UserVehicles.Add(new UserVehicle
                {
                    UserId = newUser.UserId,
                    VehicleId = existingVehicle.VehicleId
                });
            }
            else
            {
                // רכב חדש לגמרי -> יוצרים אותו ומקשרים בטבלת הגישור
                var newVehicle = new Vehicle
                {
                    LicensePlate = v.LicensePlate,
                    ModelName = v.ModelName,
                    VehicleImagePath = v.VehicleImagePath
                };
                _context.Vehicles.Add(newVehicle);
                await _context.SaveChangesAsync();

                _context.UserVehicles.Add(new UserVehicle
                {
                    UserId = newUser.UserId,
                    VehicleId = newVehicle.VehicleId
                });
            }
        }

        await _context.SaveChangesAsync();
        return Ok(newUser);
    }

    // 3. התחברות
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginInfo loginInfo)
    {
        var user = await _context.Users
            .Include(u => u.UserVehicles)
            .ThenInclude(uv => uv.Vehicle)
            .FirstOrDefaultAsync(u => u.Email == loginInfo.Email && u.Password == loginInfo.Password);

        if (user == null)
            return Unauthorized("Invalid email or password.");

        return Ok(user);
    }
}

// מחלקת עזר לקבלת נתוני ההרשמה מהאפליקציה
public class RegisterRequest
{
    public string FullName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ProfileImagePath { get; set; } = "";
    public List<Vehicle> Vehicles { get; set; } = new();
}