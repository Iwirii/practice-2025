using Xunit;
using task13;

namespace task13tests;

public class StudentTests
{
    [Fact]
    public void Serialize_IgnoresNullFields()
    {
        var student = new Student { FirstName = "Геннадий", LastName = null };

        const string testFile = "temp.json";
        student.SaveToFile(testFile);
        string json = File.ReadAllText(testFile);

        Assert.DoesNotContain("LastName", json);
        File.Delete(testFile);
    }

    [Fact]
    public void DateFormat_IsCorrect()
    {
        var student = new Student
        {
            FirstName = "Жанна",
            LastName = "Киска",
            BirthDate = new DateTime(2005, 10, 24)
        };

        const string testFile = "temp.json";
        student.SaveToFile(testFile);
        string json = File.ReadAllText(testFile);

        Assert.Contains("2005.10.24", json);
        File.Delete(testFile);
    }

    [Fact]
    public void SerializeDeserialize_PreservesData()
    {
        var original = new Student
        {
            FirstName = "Ольга",
            LastName = "Ивановна",
            Grades = new List<Subject> { new() { Name = "Математика", Grade = 5 } }
        };

        const string testFile = "temp.json";
        original.SaveToFile(testFile);
        var restored = Student.LoadFromFile(testFile);

        Assert.Equal(original.FirstName, restored.FirstName);
        Assert.Equal("Математика", restored.Grades![0].Name);
        File.Delete(testFile);
    }
}
