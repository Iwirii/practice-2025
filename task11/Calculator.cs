using System;
using System.Reflection;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace task11
{
    public interface ICalculator
    {
        int Add(int a, int b);
        int Minus(int a, int b);
        int Mul(int a, int b);
        int Div(int a, int b);
    }

    public static class CalculatorGenerator
    {
        public static ICalculator Create()
        {
            const string code = @"
                public class Calculator : task11.ICalculator
                {
                    public int Add(int a, int b) => a + b;
                    public int Minus(int a, int b) => a - b;
                    public int Mul(int a, int b) => a * b;
                    public int Div(int a, int b) => a / b;
                }";

            var compilation = CSharpCompilation.Create("CalculatorAssembly")
                .AddReferences(
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location))
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(code))
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var stream = new MemoryStream();
            var result = compilation.Emit(stream);

            if (!result.Success)
                throw new Exception("Ошибка компиляции");

            stream.Seek(0, SeekOrigin.Begin);
            var assembly = Assembly.Load(stream.ToArray());
            return (ICalculator)Activator.CreateInstance(assembly.GetType("Calculator")!)!;
        }
    }
}
