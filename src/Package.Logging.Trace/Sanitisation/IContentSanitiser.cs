namespace Recodify.Logging.Trace.Sanitisation
{
    public interface IContentSanitiser
    {
        string Sanitise(string source);
    }
}