namespace PowerDeploy.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Fmt(this string text, params object[] args)
        {
            return string.Format(text, args);
        } 
    }
}