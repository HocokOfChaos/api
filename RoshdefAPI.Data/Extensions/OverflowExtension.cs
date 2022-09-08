namespace RoshdefAPI.Data.Extensions
{
    public static class OverflowExtension
    {
        public static bool IsAdditionWillCauseOverflow(this int b, int val)
        {
            return int.MaxValue - b < val;
        }

        public static bool IsSubtractionWillCauseOverflow(this int b, int val)
        {
            return b - int.MinValue < val;
        }

        public static bool IsAdditionWillCauseOverflow(this uint b, uint val)
        {
            return uint.MaxValue - b < val;
        }

        public static bool IsSubtractionWillCauseOverflow(this uint b, uint val)
        {
            return b - uint.MinValue < val;
        }

        public static bool IsAdditionWillCauseOverflow(this ulong b, ulong val)
        {
            return ulong.MaxValue - b < val;
        }

        public static bool IsSubtractionWillCauseOverflow(this ulong b, ulong val)
        {
            return b - ulong.MinValue < val;
        }
    }
}
