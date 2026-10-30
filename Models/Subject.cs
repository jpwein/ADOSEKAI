namespace SekaiADO24.Models;

public sealed class Subject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Credits { get; set; }
    public List<Grade> Grades { get; set; } = new();
}
