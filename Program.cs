using Microsoft.EntityFrameworkCore;
using SekaiADO24.Data;
using SekaiADO24.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddDbContext<CollegeDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();

app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
    context.Response.ContentType = "application/json; charset=utf-8";
    await context.Response.WriteAsJsonAsync(new
    {
        error = "CollegeDB недоступна",
        message = "Проверьте, что база CollegeDB существует и пользователь имеет к ней доступ."
    });
}));

app.UseDefaultFiles();
app.UseStaticFiles();

await CollegeDbInitializer.EnsureDatabaseAsync(connectionString, app.Logger);
await CollegeDbSeeder.SeedAsync(app.Services, app.Logger);

app.MapGet("/sexai", () => Results.Ok(new
{
    name = "SekaiADO 24",
    description = "CollegeDB CRUD API",
    routes = new[] { "/api/students", "/api/groups", "/api/subjects", "/api/teachers", "/api/grades" }
}));

app.MapGet("/api/students", async (CollegeDbContext db, CancellationToken token) =>
    await db.Students.AsNoTracking().Include(item => item.Group).Include(item => item.Grades).ToListAsync(token));
app.MapGet("/api/students/{id:int}", async (int id, CollegeDbContext db, CancellationToken token) =>
    await db.Students.AsNoTracking().Include(item => item.Group).Include(item => item.Grades)
        .FirstOrDefaultAsync(item => item.Id == id, token) is { } item ? Results.Ok(item) : Results.NotFound());
app.MapPost("/api/students", async (Student input, CollegeDbContext db, CancellationToken token) =>
{
    db.Students.Add(input);
    await db.SaveChangesAsync(token);
    return Results.Created($"/api/students/{input.Id}", input);
});
app.MapPut("/api/students/{id:int}", async (int id, Student input, CollegeDbContext db, CancellationToken token) =>
{
    var item = await db.Students.FindAsync(new object[] { id }, token);
    if (item is null) return Results.NotFound();
    item.FirstName = input.FirstName; item.LastName = input.LastName; item.Age = input.Age; item.Course = input.Course; item.GroupId = input.GroupId;
    await db.SaveChangesAsync(token);
    return Results.NoContent();
});
app.MapDelete("/api/students/{id:int}", async (int id, CollegeDbContext db, CancellationToken token) =>
{
    var item = await db.Students.FindAsync(new object[] { id }, token);
    if (item is null) return Results.NotFound();
    db.Students.Remove(item); await db.SaveChangesAsync(token); return Results.NoContent();
});

app.MapGet("/api/groups", async (CollegeDbContext db, CancellationToken token) =>
    await db.Groups.AsNoTracking().Include(item => item.Students).ToListAsync(token));
app.MapGet("/api/groups/{id:int}", async (int id, CollegeDbContext db, CancellationToken token) =>
    await db.Groups.AsNoTracking().Include(item => item.Students).FirstOrDefaultAsync(item => item.Id == id, token) is { } item ? Results.Ok(item) : Results.NotFound());
app.MapPost("/api/groups", async (Group input, CollegeDbContext db, CancellationToken token) =>
{
    db.Groups.Add(input); await db.SaveChangesAsync(token); return Results.Created($"/api/groups/{input.Id}", input);
});
app.MapPut("/api/groups/{id:int}", async (int id, Group input, CollegeDbContext db, CancellationToken token) =>
{
    var item = await db.Groups.FindAsync(new object[] { id }, token); if (item is null) return Results.NotFound();
    item.Name = input.Name; item.Direction = input.Direction; await db.SaveChangesAsync(token); return Results.NoContent();
});
app.MapDelete("/api/groups/{id:int}", async (int id, CollegeDbContext db, CancellationToken token) =>
{
    var item = await db.Groups.FindAsync(new object[] { id }, token); if (item is null) return Results.NotFound();
    db.Groups.Remove(item); await db.SaveChangesAsync(token); return Results.NoContent();
});

app.MapGet("/api/subjects", async (CollegeDbContext db, CancellationToken token) => await db.Subjects.AsNoTracking().Include(item => item.Grades).ToListAsync(token));
app.MapGet("/api/subjects/{id:int}", async (int id, CollegeDbContext db, CancellationToken token) =>
    await db.Subjects.AsNoTracking().Include(item => item.Grades).FirstOrDefaultAsync(item => item.Id == id, token) is { } item ? Results.Ok(item) : Results.NotFound());
app.MapPost("/api/subjects", async (Subject input, CollegeDbContext db, CancellationToken token) =>
{
    db.Subjects.Add(input); await db.SaveChangesAsync(token); return Results.Created($"/api/subjects/{input.Id}", input);
});
app.MapPut("/api/subjects/{id:int}", async (int id, Subject input, CollegeDbContext db, CancellationToken token) =>
{
    var item = await db.Subjects.FindAsync(new object[] { id }, token); if (item is null) return Results.NotFound();
    item.Name = input.Name; item.Credits = input.Credits; await db.SaveChangesAsync(token); return Results.NoContent();
});
app.MapDelete("/api/subjects/{id:int}", async (int id, CollegeDbContext db, CancellationToken token) =>
{
    var item = await db.Subjects.FindAsync(new object[] { id }, token); if (item is null) return Results.NotFound();
    db.Subjects.Remove(item); await db.SaveChangesAsync(token); return Results.NoContent();
});

app.MapGet("/api/teachers", async (CollegeDbContext db, CancellationToken token) => await db.Teachers.AsNoTracking().Include(item => item.Subject).Include(item => item.Grades).ToListAsync(token));
app.MapGet("/api/teachers/{id:int}", async (int id, CollegeDbContext db, CancellationToken token) =>
    await db.Teachers.AsNoTracking().Include(item => item.Subject).Include(item => item.Grades).FirstOrDefaultAsync(item => item.Id == id, token) is { } item ? Results.Ok(item) : Results.NotFound());
app.MapPost("/api/teachers", async (Teacher input, CollegeDbContext db, CancellationToken token) =>
{
    db.Teachers.Add(input); await db.SaveChangesAsync(token); return Results.Created($"/api/teachers/{input.Id}", input);
});
app.MapPut("/api/teachers/{id:int}", async (int id, Teacher input, CollegeDbContext db, CancellationToken token) =>
{
    var item = await db.Teachers.FindAsync(new object[] { id }, token); if (item is null) return Results.NotFound();
    item.FirstName = input.FirstName; item.LastName = input.LastName; item.Email = input.Email; item.SubjectId = input.SubjectId; await db.SaveChangesAsync(token); return Results.NoContent();
});
app.MapDelete("/api/teachers/{id:int}", async (int id, CollegeDbContext db, CancellationToken token) =>
{
    var item = await db.Teachers.FindAsync(new object[] { id }, token); if (item is null) return Results.NotFound();
    db.Teachers.Remove(item); await db.SaveChangesAsync(token); return Results.NoContent();
});

app.MapGet("/api/grades", async (CollegeDbContext db, CancellationToken token) =>
    await db.Grades.AsNoTracking().Include(item => item.Student).Include(item => item.Subject).Include(item => item.Teacher).ToListAsync(token));
app.MapGet("/api/grades/{id:int}", async (int id, CollegeDbContext db, CancellationToken token) =>
    await db.Grades.AsNoTracking().Include(item => item.Student).Include(item => item.Subject).Include(item => item.Teacher)
        .FirstOrDefaultAsync(item => item.Id == id, token) is { } item ? Results.Ok(item) : Results.NotFound());
app.MapPost("/api/grades", async (Grade input, CollegeDbContext db, CancellationToken token) =>
{
    db.Grades.Add(input); await db.SaveChangesAsync(token); return Results.Created($"/api/grades/{input.Id}", input);
});
app.MapPut("/api/grades/{id:int}", async (int id, Grade input, CollegeDbContext db, CancellationToken token) =>
{
    var item = await db.Grades.FindAsync(new object[] { id }, token); if (item is null) return Results.NotFound();
    item.StudentId = input.StudentId; item.SubjectId = input.SubjectId; item.TeacherId = input.TeacherId; item.Value = input.Value; item.Date = input.Date;
    await db.SaveChangesAsync(token); return Results.NoContent();
});
app.MapDelete("/api/grades/{id:int}", async (int id, CollegeDbContext db, CancellationToken token) =>
{
    var item = await db.Grades.FindAsync(new object[] { id }, token); if (item is null) return Results.NotFound();
    db.Grades.Remove(item); await db.SaveChangesAsync(token); return Results.NoContent();
});

app.Run();
