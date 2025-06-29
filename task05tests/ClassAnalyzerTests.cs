using Xunit;
using Moq;
using task05;

public class TestClass
{
    public int PublicField;
    private string? _privateField;
    public int Property { get; set; }

    public void Method() { }
    public void MethodWithParams(int first, string second) { }
}

[Serializable]
public class AttributedClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods();

        Assert.Contains("Method", methods);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();

        Assert.Contains("_privateField", fields);
    }

    [Fact]
    public void GetMethodParams_ShouldReturnCorrectParameters()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var correct = new List<string> { "first", "second" };
        var with_params = analyzer.GetMethodParams("MethodWithParams").ToList();

        Assert.Equal(correct, with_params);

        var without_params = analyzer.GetMethodParams("Method");
        Assert.Empty(without_params);
    }

    [Fact]
    public void GetProperties_ReturnsCorrectProperties()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var properties = analyzer.GetProperties();

        Assert.Contains("Property", properties);
    }

    [Fact]
    public void HasAttribute_ShouldReturnTrue()
    {
        var analyzer = new ClassAnalyzer(typeof(AttributedClass));
        bool result = analyzer.HasAttribute<SerializableAttribute>();

        Assert.True(result);
    }
}
