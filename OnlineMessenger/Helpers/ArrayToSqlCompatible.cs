namespace OnlineMessenger.Helpers
{
    public class ArrayToSqlCompatible
    {
        public static string Convert(string[] array)
        {
            if (array.Length < 1)
            {
                return "('')";
            }

            var result = "('";

            for (var index = 0; index < array.Length - 1; ++index)
            {
                var entry = array[index];

                result += entry + "','";
            }

            result += array[array.Length - 1] + "')";

            return result;
        }
    }
}
