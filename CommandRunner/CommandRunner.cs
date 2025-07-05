using CommandLib;
using System;
using System.Reflection;

namespace CommandRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Файлы не найдены");
                return;
            }

            string dllPath = args[0];
            Assembly commandsDll = Assembly.LoadFrom(dllPath);

            if (args.Length == 2)
            {
                string folderPath = args[1];
                SizeCommand(commandsDll, folderPath);
            }
            else if (args.Length == 3)
            {
                string folderPath = args[1];
                string fileMask = args[2];
                FindCommand(commandsDll, folderPath, fileMask);
            }
        }

        static void SizeCommand(Assembly dll, string folder)
        {
            Type sizeCommandType = dll.GetType("FileSystemCommands.DirectorySizeCommand");

            ICommand command = (ICommand)Activator.CreateInstance(sizeCommandType, folder);
            command.Execute();

            long size = (long)sizeCommandType.GetProperty("Size").GetValue(command);
            Console.WriteLine($"Размер папки {folder}: {size} байт");
        }

        static void FindCommand(Assembly dll, string folder, string mask)
        {
            Type findCommandType = dll.GetType("FileSystemCommands.FindFilesCommand");

            ICommand command = (ICommand)Activator.CreateInstance(findCommandType, folder, mask);
            command.Execute();

            var files = (System.Collections.Generic.List<string>)findCommandType.GetProperty("Results").GetValue(command);
            Console.WriteLine($"Найдено файлов: {files.Count}");
            foreach (string file in files)
            {
                Console.WriteLine(file);
            }
        }
    }
}
