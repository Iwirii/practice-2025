using Xunit;
using System.Reflection;
using PluginSystem;

namespace task10tests
{
    public class PluginLoaderTests
    {
        [Fact]
        public void LoadsAndExecutesPlugins()
        {
            var loader = new PluginLoader();
            var pluginsFolder = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                "TestPlugins");

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            loader.LoadPlugins(pluginsFolder);
            loader.ExecuteAll();

            var output = consoleOutput.ToString().Split(Environment.NewLine);
            Assert.Contains("Плагин 2 выполнен", output[0]);
            Assert.Contains("Плагин 1 выполнен после Плагина 2", output[1]);
        }
    }
}
