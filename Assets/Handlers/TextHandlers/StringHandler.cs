namespace Assets.Handlers.TextHandlers
{
    public class StringHandler
    {
        public static string FirstCharToLower(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return char.ToLower(input[0]) + input.Substring(1);
        }
    }
}
