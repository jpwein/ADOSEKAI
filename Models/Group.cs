namespace SekaiADO24.Models;

public sealed class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public List<Student> Students { get; set; } = new();
}
