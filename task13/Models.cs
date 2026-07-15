using System.Text.Json.Serialization;

namespace task13;

public class Subject
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("grade")]
    public int Grade { get; set; }
}

public class Student
{
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("birthDate")]
    public DateTime BirthDate { get; set; }

    [JsonPropertyName("grades")]
    public List<Subject> Grades { get; set; } = new();

    [JsonIgnore]
    public int FullNameLength => (FirstName + LastName).Length;
}