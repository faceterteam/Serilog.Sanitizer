namespace Avalab.Serilog.Sanitizer
{
    public interface ISanitizingFormatRule
    {
        string Sanitize(string content);
    }
}
