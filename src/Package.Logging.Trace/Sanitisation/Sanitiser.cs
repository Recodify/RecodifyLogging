namespace Recodify.Logging.Trace.Sanitisation
{
    public class Sanitiser : ISanitiser
    {
	    private readonly IContentSanitiser contentSanitiser;
	    private readonly IUrlSanitiser urlSanitiser;

        public Sanitiser() : this(new ContentSanitiser(), new UrlSanitiser())
        {
        }

        public Sanitiser(IContentSanitiser contentSanitiser, IUrlSanitiser urlSanitiser)
        {
            this.contentSanitiser = contentSanitiser;
            this.urlSanitiser = urlSanitiser;
        }

	    public string Sanitise(string input)
	    {
	        var sanitised = SanitiseJsonPassword(input);
            return SanitiseUrl(sanitised);
        }

	    private string SanitiseJsonPassword(string content)
		{
            return contentSanitiser.Sanitise(content);
		}

        private string SanitiseUrl(string url)
        {
            return urlSanitiser.Sanitise(url);
        }
	}
}