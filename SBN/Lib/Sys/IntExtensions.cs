namespace SBN.Lib.Sys
{
    public static class IntExtensions
    {
        public static bool Between(this int compare, int low, int high)
        {
            return compare >= low && compare <= high;
        }
    }
}
