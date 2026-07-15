using System.Text.Json;

namespace task13;

public static class StudentSerializer
{
        
    private static JsonSerializerOptions CreateOptions() => new()
    {
        WriteIndented = true,                                 
        DefaultIgnoreCondition = System.Text.Json.Serialization
            .JsonIgnoreCondition.WhenWritingNull,             
        Converters = { new DateOnlyConverter() }             
    };

    public static string Serialize(Student student)
        => JsonSerializer.Serialize(student, CreateOptions());

    public static Student Deserialize(string json)
    {
        var student = JsonSerializer.Deserialize<Student>(json, CreateOptions())
            ?? throw new InvalidOperationException("Не удалось десериализовать JSON.");

        Validate(student);
        return student;
    }

    public static void SaveToFile(Student student, string path)
        => File.WriteAllText(path, Serialize(student));

    public static Student LoadFromFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Файл не найден", path);

        return Deserialize(File.ReadAllText(path));
    }

    private static void Validate(Student student)
    {
        if (string.IsNullOrWhiteSpace(student.FirstName))
            throw new ArgumentException("Имя студента не может быть пустым.");

        if (string.IsNullOrWhiteSpace(student.LastName))
            throw new ArgumentException("Фамилия студента не может быть пустой.");

        if (student.BirthDate > DateTime.Now)
            throw new ArgumentException("Дата рождения не может быть в будущем.");
    }
}