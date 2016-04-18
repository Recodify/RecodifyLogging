namespace Recodify.Logging.WebApi
{
    public class HttpContext : IContext
    {
        private readonly UrlContextResolver contextResolver;

        public HttpContext()
        {
            contextResolver = new UrlContextResolver();
        }

        public string GetContextString()
        {
            if (System.Web.HttpContext.Current == null)
            {
                return string.Empty;
            }

            var context = GetUrlContext();

            return $"http.{System.Web.HttpContext.Current.Request.HttpMethod}.{context}";
        }

        public string GetFullUrlWithMethod()
        {
            var currentContext = System.Web.HttpContext.Current;
            if (currentContext == null)
            {
                return "Unable to determine url";
            }

            return string.Format("{0} {1}", currentContext.Request.HttpMethod, currentContext.Request.Url);
        }

        private string GetUrlContext()
        {
            return contextResolver.Resolve(System.Web.HttpContext.Current.Request.Url.LocalPath, System.Web.HttpContext.Current.Request.HttpMethod);
        }
    }
}