using System;
using System.Reflection;

namespace task07;
public class DisplayNameAttribute : Attribute
{
    public string DisplayName { get; }
    public DisplayNameAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class VersionAttribute : Attribute
{
    public int Major { get; }
    public int Minor { get; }

    public VersionAttribute(int major, int minor) => (Major, Minor) = (major, minor);
}

[DisplayName("Пример класса")]
[Version(1, 0)]
public class SampleClass
{
    [DisplayName("Числовое свойство")]
    public int Number { get; set; }

    [DisplayName("Тестовый метод")]
    public void TestMethod()
    {

    }
}

public static class ReflectionHelper
{
    public static void PrintTypeInfo(Type type)
    {
        if (type.GetCustomAttribute<DisplayNameAttribute>() is { } displayName)
            Console.WriteLine($"Отображаемое имя: {displayName.DisplayName}");
        
        if (type.GetCustomAttribute<VersionAttribute>() is { } version)
            Console.WriteLine($"Версия: {version.Major}.{version.Minor}");

        Console.WriteLine("Методы:");
        foreach (var method in type.GetMethods())
            if (method.GetCustomAttribute<DisplayNameAttribute>() is { } methodName)
                Console.WriteLine($"{method.Name} ({methodName.DisplayName})");

        Console.WriteLine("Свойства:");
        foreach (var property in type.GetProperties())
            if (property.GetCustomAttribute<DisplayNameAttribute>() is { } propName)
                Console.WriteLine($"{property.Name} ({propName.DisplayName})");
    }
}
