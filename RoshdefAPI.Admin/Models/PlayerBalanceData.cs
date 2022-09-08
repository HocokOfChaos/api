using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RoshdefAPI.Admin.Models.DTO
{
    public class PlayerBalanceData
    {
        [Required, BindProperty(Name = "steam-id")]
        public ulong SteamID { get; set; } = 0;
        [Required, BindProperty(Name = "crystals")]
        public int Crystals { get; set; } = 0;
        [Required, BindProperty(Name = "coins")]
        public int Coins { get; set; } = 0;
        [Required, BindProperty(Name = "soul-stones")]
        public int SoulStones { get; set; } = 0;
    }
}
