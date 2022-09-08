using RoshdefAPI.Data.Models.Core;

namespace RoshdefAPI.Admin.Models
{
    public class UserRole : IDataObject
    {
        public ulong ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
    }
}
