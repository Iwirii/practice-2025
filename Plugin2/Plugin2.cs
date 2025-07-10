using PluginSystem;

[Plugin] 
public class Plugin2 : IPlugin
{
    public void Execute()
    {
        Console.WriteLine("Плагин 2 выполнен");
    }
}
