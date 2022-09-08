using System.ComponentModel.DataAnnotations;

namespace RoshdefAPI.Data.Models.Core
{
    public interface IDataObject
    {
        [Key]
        public ulong ID { get; set; }
    }
}