namespace SyslogSharp.Writers;

public interface ISyslogWriter
{
    Task WriteSyslogMessage(SyslogEventArgs syslogEventArgs);
}