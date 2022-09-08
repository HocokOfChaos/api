namespace RoshdefAPI.Shared.Services.Core
{
    public interface IAPIKeyValidator
    {
        public bool IsKeyValid(string apiKey);
    }
}
