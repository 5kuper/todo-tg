namespace Utilities.Text
{
    public static class TextHelper
    {
        public static string PadCenter(this string text, int length)
        {
            const char dot = '·';
            const char fill = '-';

            int padding = length - text.Length - 2;
            int left = padding / 2;
            int right = padding - left;

            return dot + new string(fill, left) + text + new string(fill, right) + dot;
        }
    }
}
