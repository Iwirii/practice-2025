using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;
public class Subject
{
    public string? Name { get; set; }
    public int Grade { get; set; }
}

public class Student
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    [JsonConverter(typeof(DateFormatConverter))]
    public DateTime BirthDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Subject>? Grades { get; set; }

    public void SaveToFile(string filePath)
    {
        File.WriteAllText(filePath, JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        }));
    }

    public static Student LoadFromFile(string filePath)
    {
        var student = JsonSerializer.Deserialize<Student>(File.ReadAllText(filePath));
        if (student == null || string.IsNullOrWhiteSpace(student.FirstName) || string.IsNullOrWhiteSpace(student.LastName))
        {
            throw new JsonException("Ошибка: FirstName и LastName обязательны.");
        }
        return student;
    }


}

public class DateFormatConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateTime.Parse(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString("yyyy.MM.dd"));
}
