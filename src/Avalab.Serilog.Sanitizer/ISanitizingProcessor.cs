namespace Avalab.Serilog.Sanitizer
{
    interface ISanitizingProcessor
    {
        string Process(string content);
    }
}
