namespace RoshdefAPI.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NotExistsInDatabaseTable : Attribute
    {
    }
}
