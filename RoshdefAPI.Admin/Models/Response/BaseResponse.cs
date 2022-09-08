using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Admin.Models.Response
{
    public class BaseResponse
    {
        [Required, JsonPropertyName("success")]
        public bool Success { get; set; } = false;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("error")]
        public string? Error
        {
            get
            {
                if (Success == true)
                {
                    return null;
                }
                return _error;
            }
            set => _error = value;
        }

        private string? _error = "Unknown error";
    }

    public class BaseResponse<T> : BaseResponse where T : new()
    {
        [Required, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("data")]
        public T? Data
        {
            get
            {
                if (Success == true)
                {
                    return _data;
                }
                return default;
            }
            set => _data = value;
        }

        private T? _data = new();
    }
}
