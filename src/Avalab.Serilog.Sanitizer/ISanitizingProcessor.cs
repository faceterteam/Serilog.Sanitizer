namespace Avalab.Serilog.Sanitizer
{
    interface ISanitizingProcessor
    {
        string Sanitize(string content);

        string Sanitize(string content, string key);
    }
}
