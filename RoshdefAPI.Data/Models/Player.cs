using RoshdefAPI.Data.Attributes;
using RoshdefAPI.Data.Constants;
using RoshdefAPI.Data.Models.Core;
using RoshdefAPI.Data.Extensions;
using System.Security.Cryptography;
using System.Text;

namespace RoshdefAPI.Data.Models
{
    public class Player : IDataObject
    {
        public ulong ID { get; set; }
        public ulong SteamID { get; }
        [NotExistsInDatabaseTable]
        public IReadOnlyList<PlayerItem> Items => _items.AsReadOnly();

        public int Crystals { get; private set; } = 0;
        public int Coins { get; private set; } = 0;
        public int SoulStones { get; private set; } = 0;
        public string Payment { get; }
        public DateTime LastDailyQuestDateTime { get; set; } = DateTime.Now.AddDays(-1).Date;
        public DateTime LastDailyRewardDateTime { get; set; } = DateTime.Now.AddDays(-1).Date;
        [NotExistsInDatabaseTable]
        public bool IsDailyQuestAvailable => DateTime.Compare(DateTime.Now.Date, LastDailyQuestDateTime.Date) > 0;
        [NotExistsInDatabaseTable]
        public bool IsDailyRewardAvailable => DateTime.Compare(DateTime.Now.Date, LastDailyRewardDateTime.Date) > 0;
        public uint CurrentDailyRewardDay { get; set; } = 0;

        private readonly List<PlayerItem> _items = new();

        // Required for auto mapper, dapper, etc
        public Player() : this(PlayerConstants.InvalidSteamID)
        {

        }

        public Player(ulong steamID)
        {
            SteamID = steamID;
            Payment = GeneratePaymentURL();
        }

        public (bool isAdded, PlayerItem resultItem) AddItem(PlayerItem item, TimeSpan? duration)
        {
            ArgumentNullException.ThrowIfNull(item);
            PlayerItem? resultItem = Items.Where(playerItem => playerItem.ItemID.Equals(item.ItemID) && (duration.HasValue ? playerItem.ExpireDate.HasValue : !playerItem.ExpireDate.HasValue)).FirstOrDefault();
            if (resultItem == null)
            {
                resultItem = item;
                resultItem.ExpireDate = duration.HasValue ? DateTime.Now.Add(duration.Value) : null;
                _items.Add(resultItem);
                return (true, resultItem);
            }
            else
            {
                if (duration.HasValue && resultItem.ExpireDate.HasValue)
                {
                    resultItem.ExpireDate = resultItem.ExpireDate.Value.Add(duration.Value);
                }
                resultItem.Count += item.Count;
                return (false, resultItem);
            }
        }

        /// <summary>
        /// Consume specified amount of item (item must exists in player inventory for any effect)
        /// </summary>
        /// <param name="item">Reference to item in player inventory</param>
        /// <param name="amount">Desired amount of item to consume</param>
        /// <returns>True if item removed from player inventory</returns>
        public bool ConsumeItem(PlayerItem item, uint amount)
        {
            ArgumentNullException.ThrowIfNull(item);
            if (item.Count.IsSubtractionWillCauseOverflow(amount))
            {
                _items.Remove(item);
                return true;
            }
            else
            {
                item.Count -= amount;
                if (item.Count == 0)
                {
                    _items.Remove(item);
                    return true;
                }
                return false;
            }
        }

        public void SetBalance(int crystals, int coins, int soulStones)
        {
            Crystals = Math.Max(0, crystals);
            Coins = Math.Max(0, coins);
            SoulStones = Math.Max(0, soulStones);
        }

        public void AddBalance(int crystals, int coins, int soulStones)
        {
            AddCrystals(crystals);
            AddCoins(coins);
            AddSoulStones(soulStones);
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
        private string GeneratePaymentURL()
        {
            using SHA256 sha256 = SHA256.Create();
            var secret = string.Format("{0}-{1}", SteamID, DateTime.Now.ToString());
            byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(secret));
            string hashString = string.Empty;
            foreach (byte x in hashValue)
            {
                hashString += string.Format("{0:x2}", x);
            }
            return hashString;
        }
    }
}
