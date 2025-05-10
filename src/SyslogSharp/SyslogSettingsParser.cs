using System.Text.Json;

using Microsoft.Extensions.Logging;

namespace SyslogSharp;
internal sealed class SyslogSettingsParser
{
    private readonly string _fileName;
    private readonly ILogger<SyslogSettingsParser> _logger;

    public SyslogSettingsParser(string fileName, ILogger<SyslogSettingsParser> logger)
    {
        _fileName = fileName;
        _logger = logger;
    }

    public async Task<SyslogSettings> ParseAsync(CancellationToken cancellationToken = default)
    {
        var file = new FileInfo(_fileName);
        var exists = file.Exists;
        var settings = new SyslogSettings();
        using var fileStream = new FileStream(_fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        if (!exists)
        {
            await JsonSerializer.SerializeAsync(fileStream, settings, cancellationToken: cancellationToken);
            return settings;
        }

        settings = await JsonSerializer.DeserializeAsync<SyslogSettings>(fileStream, cancellationToken: cancellationToken);
        return settings ?? throw new InvalidOperationException("Settings cannot be null");
    }
}
