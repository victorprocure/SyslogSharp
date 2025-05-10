using CommandLine;

namespace SyslogSharp;
internal class ConsoleOptions
{
    [Option('s', "settings", Required = false, HelpText = "Settings File Name, can be name, full path or relative path. If the file does not exist it will be created")]
    public string SettingsFile { get; set; } = "settings.json";

    [Option('v', "verbose", Required = false, HelpText = "Set logging to verbose in console window")]
    public bool Verbose { get; set; } = false;
}
