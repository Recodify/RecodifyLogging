namespace Recodify.Logging.Common
{
    public interface IContext
    {
        string GetContextString();
        string GetFullUrlWithMethod();
        string GetSessionId();
		string GetClientIp();
	}
}