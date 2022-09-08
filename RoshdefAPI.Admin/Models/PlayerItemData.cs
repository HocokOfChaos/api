using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RoshdefAPI.Admin.Models.DTO
{
    public class PlayerItemData
    {
        [Required, BindProperty(Name = "item-steam-id")]
        public ulong SteamID { get; set; } = 0;
        [Required, BindProperty(Name = "item-id")]
        public ulong ItemID { get; set; } = 0;
        [Required, BindProperty(Name = "item-count")]
        public uint Count { get; set; } = 0;
        [BindProperty(Name = "item-expire-date")]
        public DateTime? ExpireDate { get; set; } = null;
    }
}
