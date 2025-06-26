using Xunit;
using task02;

namespace task02tests;

public class StudentServiceTests
{
    private readonly List<Student> _testStudents;
    private readonly StudentService _service;

    public StudentServiceTests()
    {
        _testStudents = new List<Student>
        {
            new() { Name = "Иван", Faculty = "ФИТ", Grades = new List<int> { 5, 4, 5 } },
            new() { Name = "Анна", Faculty = "ФИТ", Grades = new List<int> { 3, 4, 3 } },
            new() { Name = "Петр", Faculty = "Экономика", Grades = new List<int> { 5, 5, 5 } },
            new() { Name = "Николай", Faculty = "МО", Grades = new List<int> { 3, 3, 3 } },
            new() { Name = "Жанна", Faculty = "МО", Grades = new List<int> { } }
        };
        _service = new StudentService(_testStudents);
    }

    [Fact]
    public void GetStudentsByFaculty_ReturnsCorrectStudents()
    {
        var result = _service.GetStudentsByFaculty("ФИТ").ToList();
        Assert.Equal(2, result.Count);
        Assert.True(result.All(s => s.Faculty == "ФИТ"));
    }

    [Fact]
    public void GetStudentsWithMinAverageGrade_ReturnsStudentsWithTheRequiredGradePointAverage()
    {
        var result = _service.GetStudentsWithMinAverageGrade(3.8).ToList();
        Assert.Equal(2, result.Count);
        Assert.True(result.All(s => s.Grades.Average() >= 3.8));
    }

    [Fact]
    public void GetStudentsOrderedByName_ReturnsStudentsSortedAlphabetically()
    {
        var result = _service.GetStudentsOrderedByName().ToList();
        Assert.Equal("Анна", result[0].Name);
        Assert.Equal("Жанна", result[1].Name);
        Assert.Equal("Иван", result[2].Name);
        Assert.Equal("Николай", result[3].Name);
        Assert.Equal("Петр", result[4].Name);
    }

    [Fact]
    public void GroupStudentsByFaculty_ReturnsStudentsFromTheSameFaculty()
    {
        var result = _service.GroupStudentsByFaculty();
        Assert.Equal(3, result.Count);
        Assert.Equal(2, result["ФИТ"].Count());
        Assert.Single(result["Экономика"]);
        Assert.Equal(2, result["МО"].Count());
    }

    [Fact]
    public void GetFacultyWithHighestAverageGrade_ReturnsCorrectFaculty()
    {
        var result = _service.GetFacultyWithHighestAverageGrade();
        Assert.Equal("Экономика", result);
    }
}
