using System.Text.RegularExpressions;

namespace OnlineMessanger.Helpers
{
    public class TagCleaner
    {
        public static string CleanUp(string contents)
        {
            var expressionForCleaning = new Regex("<[^>]*>");

            var result = expressionForCleaning.Replace(contents, "");

            return result;
        }
    }
}
