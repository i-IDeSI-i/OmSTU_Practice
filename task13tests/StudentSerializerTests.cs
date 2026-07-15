using Xunit;
using task13;

public class StudentSerializerTests
{
    private static Student CreateSample() => new()
    {
        FirstName = "Иван",
        LastName = "Петров",
        BirthDate = new DateTime(2000, 5, 15),
        Grades = new List<Subject>
        {
            new() { Name = "Математика", Grade = 5 },
            new() { Name = "Физика", Grade = 4 }
        }
    };

    [Fact]
    public void Serialize_ProducesJsonWithCustomDateFormat()
    {
        var json = StudentSerializer.Serialize(CreateSample());

        Assert.Contains("2000-05-15", json); 
        Assert.Contains("\"firstName\"", json);  
        Assert.DoesNotContain("FullNameLength", json);
    }

    [Fact]
    public void SerializeDeserialize_RoundTrip_PreservesData()
    {
        var original = CreateSample();
        var json = StudentSerializer.Serialize(original);
        var restored = StudentSerializer.Deserialize(json);

        Assert.Equal(original.FirstName, restored.FirstName);
        Assert.Equal(original.LastName, restored.LastName);
        Assert.Equal(original.BirthDate, restored.BirthDate);
        Assert.Equal(2, restored.Grades.Count);
        Assert.Equal("Математика", restored.Grades[0].Name);
    }

   [Fact]
    public void Deserialize_EmptyFirstName_ThrowsValidation()
    {
        var json = @"{
            ""firstName"": """",
            ""lastName"": ""Петров"",
            ""birthDate"": ""2000-05-15"",
            ""grades"": []
        }";

        Assert.Throws<ArgumentException>(() => StudentSerializer.Deserialize(json));
    }

    [Fact]
    public void Deserialize_FutureBirthDate_ThrowsValidation()
    {
        var student = CreateSample();
        student.BirthDate = DateTime.Now.AddYears(1);
        var json = StudentSerializer.Serialize(student);

        Assert.Throws<ArgumentException>(() => StudentSerializer.Deserialize(json));
    }

    [Fact]
    public void SaveAndLoad_File_RoundTrip()
    {
        var path = Path.Combine(Path.GetTempPath(), $"student_{Guid.NewGuid()}.json");
        var original = CreateSample();

        try
        {
            StudentSerializer.SaveToFile(original, path);
            var loaded = StudentSerializer.LoadFromFile(path);

            Assert.Equal(original.FirstName, loaded.FirstName);
            Assert.Equal(original.Grades.Count, loaded.Grades.Count);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}