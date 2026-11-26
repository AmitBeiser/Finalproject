using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using ParkinServer.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. הפעלת Controllers + מניעת לולאות אינסופיות בהמרה ל-JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddOpenApi();

// 2. חיבור מסד הנתונים SQLite
builder.Services.AddDbContext<ParkinDbContext>(options =>
    options.UseSqlite("Data Source=parkin.db"));

var app = builder.Build();

// 3. יצירה אוטומטית של קובץ מסד הנתונים והטבלאות בעליית השרת
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ParkinDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// מאפשר בהמשך להציג תמונות פרופיל, רכבים וחניות מתוך השרת
app.UseStaticFiles();

// בזמן פיתוח מקומי עדיף לעבוד ב-HTTP כדי שאפליקציית ה-MAUI לא תיחסם על שגיאות תעודת SSL
// app.UseHttpsRedirection();

// 4. חיבור כל ה-Controllers (כמו UsersController) לכתובות ה-API
app.MapControllers();

app.Run();