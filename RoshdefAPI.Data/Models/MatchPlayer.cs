using RoshdefAPI.Data.Constants;
using RoshdefAPI.Data.Models.Core;
using RoshdefAPI.Data.Extensions;
using System.Text.Json;

namespace RoshdefAPI.Data.Models
{
    public class MatchPlayer : IDataObject
    {
        public ulong ID { get; set; }
        public ulong MatchID { get; }
        public ulong SteamID { get; }
        public uint GoldPerMinute { get; set; }
        public uint ExpiriencePerMinute { get; set; }
        public ulong Networth { get; set; }
        public uint CreepsKilled { get; set; }
        public uint BossesKilled { get; set; }
        public ulong DamageDone { get; set; }
        public ulong DamageTaken { get; set; }
        public ulong HealingDone { get; set; }
        public ulong LifestealDone { get; set; }
        public string QuestsFinished { get; private set; } = JsonSerializer.Serialize(new Dictionary<string, string>());
        public string ItemsBuild { get; private set; } = JsonSerializer.Serialize(new Dictionary<string, string>());
        public int Crystals { get; private set; } = 0;
        public int Coins { get; private set; } = 0;
        public int SoulStones { get; private set; } = 0;

        // Required for auto mapper, dapper, etc
        public MatchPlayer() : this(PlayerConstants.InvalidSteamID, MatchConstants.InvalidMatchID)
        {

        }
        public MatchPlayer(ulong steamID, ulong matchID)
        {
            SteamID = steamID;
            MatchID = matchID;
        }

        public void AddCrystals(int crystals)
        {
            if (Crystals.IsAdditionWillCauseOverflow(crystals))
            {
                Crystals = int.MaxValue;
            }
            else
            {
                Crystals = Math.Max(Crystals + crystals, 0);
            }
        }
        public void AddCoins(int coins)
        {
            if (Coins.IsAdditionWillCauseOverflow(coins))
            {
                Coins = int.MaxValue;
            }
            else
            {
                Coins = Math.Max(Coins + coins, 0);
            }
        }
        public void AddSoulStones(int soulStones)
        {
            if (SoulStones.IsAdditionWillCauseOverflow(soulStones))
            {
                SoulStones = int.MaxValue;
            }
            else
            {
                SoulStones = Math.Max(SoulStones + soulStones, 0);
            }
        }

        public void SetItemsBuild(Dictionary<string, string> itemsBuild)
        {
            itemsBuild ??= new Dictionary<string, string>();
            ItemsBuild = JsonSerializer.Serialize(itemsBuild);
        }

        public void SetQuestsFinished(Dictionary<string, string> quests)
        {
            quests ??= new Dictionary<string, string>();
            QuestsFinished = JsonSerializer.Serialize(quests);
        }
    }
}
