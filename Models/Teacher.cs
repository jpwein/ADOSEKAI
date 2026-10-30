namespace SekaiADO24.Models;

public sealed class Teacher
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public List<Grade> Grades { get; set; } = new();
}
