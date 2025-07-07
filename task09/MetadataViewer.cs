using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace task09
{
    public class MetadataViewer
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Ошибка: укажите путь к DLL файлу");
                return;
            }

            string dllPath = args[0];

            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"Ошибка: файл не найден");
                return;
            }

            var assembly = Assembly.LoadFrom(dllPath);
            var classTypes = assembly.GetTypes().Where(t => t.IsClass);

            foreach (var classType in classTypes)
            {
                Console.WriteLine($"\nКласс: {classType.Name}");
                
                var classAttributes = classType.GetCustomAttributes();
                if (classAttributes.Any())
                {
                    Console.WriteLine("Атрибуты класса:");
                    classAttributes.ToList().ForEach(attr => 
                        Console.WriteLine($"  {attr.GetType().Name}"));
                }

                var constructors = classType.GetConstructors();
                if (constructors.Any())
                {
                    Console.WriteLine("Конструкторы:");
                    constructors.ToList().ForEach(ctor => 
                    {
                        Console.WriteLine($"  {ctor.Name}");
                        ctor.GetParameters().ToList().ForEach(p => 
                            Console.WriteLine($"    Параметр: {p.ParameterType} {p.Name}"));
                    });
                }

                var methods = classType.GetMethods()
                    .Where(m => !m.IsSpecialName);
                
                if (methods.Any())
                {
                    Console.WriteLine("Методы:");
                    methods.ToList().ForEach(m => 
                    {
                        Console.WriteLine($"  {m.ReturnType} {m.Name}()");
                        m.GetParameters().ToList().ForEach(p => 
                            Console.WriteLine($"    Параметр: {p.ParameterType} {p.Name}"));
                    });
                }

                var properties = classType.GetProperties();
                if (properties.Any())
                {
                    Console.WriteLine("Свойства:");
                    properties.ToList().ForEach(p => 
                        Console.WriteLine($"  {p.PropertyType} {p.Name}"));
                }
            }
        }
    }
}