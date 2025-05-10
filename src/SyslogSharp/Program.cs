using CommandLine;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SyslogSharp;
using SyslogSharp.Writers;

await Parser.Default.ParseArguments<ConsoleOptions>(args)
    .WithParsedAsync(async o =>
    {
        var services = new ServiceCollection()
            .AddLogging(lo => lo.AddConsole().SetMinimumLevel(o.Verbose ? LogLevel.Debug : LogLevel.Information));
        services.AddSingleton<SyslogSettingsParser>(sp => new(o.SettingsFile, sp.GetRequiredService<ILogger<SyslogSettingsParser>>()));

        services.AddSingleton<SyslogServer>();
        services.AddSingleton<ISyslogWriter, SyslogLoggerWriter>();
        var builder = services.BuildServiceProvider();

        var server = builder.GetRequiredService<SyslogServer>();

        await server.StartServerAsync(CancellationToken.None);
        while (true)
        {
            var line = Console.ReadLine();

            if(line == "q")
            {
                break;
            }
        }

        await server.StopServerAsync(CancellationToken.None);
    });