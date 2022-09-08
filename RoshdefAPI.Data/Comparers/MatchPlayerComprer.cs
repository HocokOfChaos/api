using RoshdefAPI.Data.Models;
using System.Diagnostics.CodeAnalysis;

namespace RoshdefAPI.Data.Comparers
{
    internal class MatchPlayerComparer : IEqualityComparer<MatchPlayer>
    {
        public bool Equals(MatchPlayer? x, MatchPlayer? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.SteamID.Equals(y.SteamID);
        }

        public int GetHashCode([DisallowNull] MatchPlayer obj)
        {
            if (obj == null) return 0;
            return obj.SteamID.GetHashCode();
        }
    }
}
