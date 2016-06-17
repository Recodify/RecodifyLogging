namespace Recodify.Logging.Web
{
    public interface IContext
    {
        string GetContextString();
        string GetFullUrlWithMethod();
        string GetSessionId();
    }
}