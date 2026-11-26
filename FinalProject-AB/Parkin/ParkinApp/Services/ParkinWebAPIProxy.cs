using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ParkinApp.Models;

namespace ParkinApp.Services;

public class ParkinWebAPIProxy
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;
    public User? CurrentUser { get; set; }

    public static string BaseAddress =>
        (DeviceInfo.Platform == DevicePlatform.Android && DeviceInfo.DeviceType == DeviceType.Virtual)
            ? "http://10.0.2.2:5172/api/"
            : "http://localhost:5172/api/";

    public ParkinWebAPIProxy()
    {
        var handler = new HttpClientHandler
        {
            CookieContainer = new CookieContainer()
        };
        _client = new HttpClient(handler)
        {
            BaseAddress = new Uri(BaseAddress)
        };
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    // 1. בדיקה אם רכב כבר קיים במערכת לפי מספר רישוי
    public async Task<VehicleCheckResult?> CheckVehicleAsync(string licensePlate)
    {
        try
        {
            var response = await _client.GetAsync($"Users/CheckVehicle?licensePlate={Uri.EscapeDataString(licensePlate)}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<VehicleCheckResult>(_jsonOptions);
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CheckVehicle error: {ex.Message}");
            return null;
        }
    }

    // 2. הרשמת משתמש חדש + הרכבים שלו
    public async Task<(User? user, string? error)> RegisterAsync(User newUser)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("Users/Register", newUser);
            if (response.IsSuccessStatusCode)
            {
                var createdUser = await response.Content.ReadFromJsonAsync<User>(_jsonOptions);
                CurrentUser = createdUser;
                return (createdUser, null);
            }

            var errorText = await response.Content.ReadAsStringAsync();
            return (null, string.IsNullOrWhiteSpace(errorText) ? "Registration failed." : errorText);
        }
        catch (Exception ex)
        {
            return (null, $"Connection error: {ex.Message}");
        }
    }

    // 3. התחברות
    public async Task<(User? user, string? error)> LoginAsync(string email, string password)
    {
        try
        {
            var loginData = new { Email = email, Password = password };
            var response = await _client.PostAsJsonAsync("Users/Login", loginData);
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<User>(_jsonOptions);
                CurrentUser = user;
                return (user, null);
            }

            var errorText = await response.Content.ReadAsStringAsync();
            return (null, string.IsNullOrWhiteSpace(errorText) ? "Invalid email or password." : errorText);
        }
        catch (Exception ex)
        {
            return (null, $"Connection error: {ex.Message}");
        }
    }
}

// מחלקת עזר לקבלת פרטי רכב קיים מהשרת (עבור פיצ'ר שיתוף רכב)
public class VehicleCheckResult
{
    public int VehicleId { get; set; }
    public string LicensePlate { get; set; } = "";
    public string ModelName { get; set; } = "";
    public string VehicleImagePath { get; set; } = "";
    public string Owners { get; set; } = "";
}