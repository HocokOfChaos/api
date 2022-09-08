using RoshdefAPI.Data.Models.Core;

namespace RoshdefAPI.Admin.Models
{
    public class User : IDataObject
    {
        public ulong ID { get; set; }
        public string Login { get; set; } = string.Empty;
        public string NormalizedLogin { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
