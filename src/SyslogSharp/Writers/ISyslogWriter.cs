namespace SyslogSharp.Writers;

public interface ISyslogWriter
{
    Task WriteSyslogMessage(SyslogEvent syslogEventArgs);
}