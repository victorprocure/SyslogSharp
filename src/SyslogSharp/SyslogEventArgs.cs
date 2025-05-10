namespace SyslogSharp;

public class SyslogEventArgs : EventArgs
{
    public SyslogEventArgs(string message, string? sender, string? hostname, string? appName, string? procId, string? msgId)
    {
        Message = message;
        Sender = sender;
        Hostname = hostname;
        AppName = appName;
        ProcId = procId;
        MsgId = msgId;
    }

    public string Message { get; }
    public string? Sender { get; }
    public string? Hostname { get; }
    public string? AppName { get; }
    public string? ProcId { get; }
    public string? MsgId { get; }
}