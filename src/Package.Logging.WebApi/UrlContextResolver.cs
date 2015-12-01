using System.Collections.Generic;
using System.Linq;

namespace Collinson.Logging.WebApi
{
    public class UrlContextResolver
    {
        public string Resolve(string urlLocalPath, string httpMethod)
        {
            if (string.IsNullOrWhiteSpace(urlLocalPath))
                return "nonweb";

            var sections = urlLocalPath.Trim('/').Split('/');

            if (sections.Count() == 1)
                return sections.First() + "." + httpMethod;

            IEnumerable<string> controllerAndAction = null;
            if (IsParentChildUrl(sections))
            {
                controllerAndAction = IsEntityInstanceParentChild(sections) ? GetFirstAndLast(sections) : GetThirdAndFourth(sections);
            }
            else
                controllerAndAction = GetFirstTwo(sections);


            return (controllerAndAction.First() + "." + controllerAndAction.Last()).ToLower();
        }

        private static IEnumerable<string> GetFirstTwo(string[] sections)
        {
            return sections.Take(2);
        }

        private static IEnumerable<string> GetThirdAndFourth(string[] sections)
        {
            return sections.Skip(2).Take(2);
        }

        private static bool IsEntityInstanceParentChild(string[] sections)
        {
            int intTest;
            return int.TryParse(GetThird(sections), out intTest);
        }

        private static bool IsParentChildUrl(string[] sections)
        {
            return sections.Count() == 4;
        }

        private static string GetThird(string[] sections)
        {
            return sections.Skip(2).First();
        }

        private static IEnumerable<string> GetFirstAndLast(string[] sections)
        {
            return sections.Take(1).Concat(sections.Skip(3).Take(1));
        }
    }
}