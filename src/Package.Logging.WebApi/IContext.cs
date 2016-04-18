namespace Recodify.Logging.WebApi
{
    public interface IContext
    {
        string GetContextString();
        string GetFullUrlWithMethod();
    }
}