using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.Request
{
    public class BaseRequest
    {
        [Required, JsonPropertyName("secretkey")]
        public string APIKey { get; set; } = "api_key_not_specified";
    }

    public class BaseRequest<T> : BaseRequest where T : new()
    {
        [Required, JsonPropertyName("data")]
        public virtual T Data { get; set; } = new T();

    }
}
