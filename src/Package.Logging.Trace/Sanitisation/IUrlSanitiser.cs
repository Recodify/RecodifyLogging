namespace Recodify.Logging.Trace.Sanitisation
{
    public interface IUrlSanitiser
    {
        string Sanitise(string source);
    }
}