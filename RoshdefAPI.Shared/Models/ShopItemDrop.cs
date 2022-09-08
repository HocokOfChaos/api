namespace RoshdefAPI.Shared.Models
{
    public class ShopItemDrop
    {
        public uint ID { get; private set; }
        public float DropChance { get; private set; } = 0f;

        public ShopItemDrop(uint id, float dropChance)
        {
            ID = id;
            DropChance = dropChance;
        }
    }
}
