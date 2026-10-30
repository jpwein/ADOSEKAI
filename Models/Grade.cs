namespace SekaiADO24.Models;

public sealed class Grade
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public decimal Value { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public Student? Student { get; set; }
    public Subject? Subject { get; set; }
    public Teacher? Teacher { get; set; }
}
