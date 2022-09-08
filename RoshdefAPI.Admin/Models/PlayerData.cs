using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RoshdefAPI.Admin.Models.DTO
{
    public class PlayerData
    {
        [Required, BindProperty(Name = "steam-id")]
        public ulong SteamID { get; set; } = 0;
    }
}
