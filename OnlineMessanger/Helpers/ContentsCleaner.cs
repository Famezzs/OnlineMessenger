using System.Text.RegularExpressions;

namespace OnlineMessanger.Helpers
{
    public class TagCleaner
    {
        public static string CleanUp(string contents)
        {
            // This expression selects all substring which
            // are between "<" ">" brackets
            var expressionForCleaning = new Regex("<[^>]*>");

            var result = expressionForCleaning.Replace(contents, "");

            return result;
        }
    }
}
