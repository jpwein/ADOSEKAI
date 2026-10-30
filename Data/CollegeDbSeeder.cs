using Microsoft.EntityFrameworkCore;
using SekaiADO24.Models;

namespace SekaiADO24.Data;

public static class CollegeDbSeeder
{
    public static async Task SeedAsync(IServiceProvider services, ILogger logger)
    {
        try
        {
            await using var scope = services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<CollegeDbContext>();

            await db.Database.EnsureCreatedAsync();
            if (await db.Students.AnyAsync() || await db.Groups.AnyAsync())
            {
                logger.LogInformation("CollegeDB уже содержит данные. Начальные записи пропущены.");
                return;
            }

            var groups = new List<Group>
            {
                new() { Name = "ПИ-21", Direction = "Программная инженерия" },
                new() { Name = "ИВТ-22", Direction = "Информационные технологии" },
                new() { Name = "ПР-23", Direction = "Прикладная информатика" }
            };
            db.Groups.AddRange(groups);

            var subjects = new List<Subject>
            {
                new() { Name = "ADO.NET", Credits = 4 }, new() { Name = "Базы данных", Credits = 5 },
                new() { Name = "C# и .NET", Credits = 4 }, new() { Name = "Веб-разработка", Credits = 3 }
            };
            db.Subjects.AddRange(subjects);

            var teachers = new List<Teacher>
            {
                new() { FirstName = "П.Р.", LastName = "Латвия", Email = "pr.latvia@example.local", Subject = subjects[0] },
                new() { FirstName = "Р.П.", LastName = "Пила", Email = "rp.pila@example.local", Subject = subjects[1] },
                new() { FirstName = "А.В.", LastName = "Север", Email = "av.sever@example.local", Subject = subjects[2] }
            };
            db.Teachers.AddRange(teachers);

            var students = new List<Student>
            {
                new() { FirstName = "П.Р.", LastName = "Латвия", Age = 20, Course = 2, Group = groups[0] },
                new() { FirstName = "Р.П.", LastName = "Пила", Age = 21, Course = 2, Group = groups[0] },
                new() { FirstName = "И.С.", LastName = "Маяк", Age = 19, Course = 1, Group = groups[1] },
                new() { FirstName = "К.Д.", LastName = "Река", Age = 22, Course = 4, Group = groups[1] },
                new() { FirstName = "В.А.", LastName = "Сокол", Age = 20, Course = 2, Group = groups[2] },
                new() { FirstName = "Н.М.", LastName = "Туман", Age = 21, Course = 3, Group = groups[2] }
            };
            db.Students.AddRange(students);
            await db.SaveChangesAsync();

            db.Grades.AddRange(
                new Grade { Student = students[0], Subject = subjects[0], Teacher = teachers[0], Value = 5 },
                new Grade { Student = students[0], Subject = subjects[1], Teacher = teachers[1], Value = 4 },
                new Grade { Student = students[1], Subject = subjects[0], Teacher = teachers[0], Value = 4 },
                new Grade { Student = students[2], Subject = subjects[2], Teacher = teachers[2], Value = 5 },
                new Grade { Student = students[3], Subject = subjects[1], Teacher = teachers[1], Value = 3 },
                new Grade { Student = students[4], Subject = subjects[3], Teacher = teachers[2], Value = 4 },
                new Grade { Student = students[5], Subject = subjects[2], Teacher = teachers[0], Value = 5 });
            await db.SaveChangesAsync();

            logger.LogInformation("Добавлены демонстрационные данные: {Students} студентов, {Groups} групп, {Subjects} предмета, {Teachers} преподавателя и оценки.", students.Count, groups.Count, subjects.Count, teachers.Count);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Не удалось добавить демонстрационные данные. Проверьте подключение к CollegeDB.");
        }
    }
}
