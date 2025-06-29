using System;
using System.Reflection;
using System.Collections.Generic;
namespace task05;

public class ClassAnalyzer
{
    private Type _type;

    public ClassAnalyzer(Type type)
    {
        _type = type;
    }

    public IEnumerable<string> GetPublicMethods()
    {
        return _type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Select(m => m.Name);
    }

    public IEnumerable<string> GetMethodParams(string methodname)
    {
        MethodInfo? method = _type.GetMethod(methodname);
        if (method == null)
            return Enumerable.Empty<string>();
        ParameterInfo[] parameters = method.GetParameters();
        return parameters.Select(p => p.Name!);
    }

    public IEnumerable<string> GetAllFields()
    {
        return _type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static)
        .Select(p => p.Name);
    }

    public IEnumerable<string> GetProperties()
    {
        return _type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static)
        .Select(n => n.Name);
    }

    public bool HasAttribute<T>() where T : Attribute
    {
        return Attribute.IsDefined(_type, typeof(T));
    }
}
