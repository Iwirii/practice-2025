using PluginSystem;

[Plugin(typeof(Plugin2))]
public class Plugin1 : IPlugin
{
    public void Execute()
    {
        Console.WriteLine("Плагин 1 выполнен после Плагина 2");
    }
}
