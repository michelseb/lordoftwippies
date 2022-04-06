public enum ResourceType
{
    Food,
    Drink
}
public class Resource
{
    public ResourceType ResourceType { get; private set; }
    public IConsumable Consumable { get; private set; }
    public float Quantity { get; set; }

    public Resource(ResourceType resourceType, IConsumable consumable, float quantity)
    {
        ResourceType = resourceType;
        Consumable = consumable;
        Quantity = quantity;
    }
    public Resource(ResourceType resourceType)
    {
        ResourceType = resourceType;
        Quantity = float.MaxValue;
    }
    public Resource(ResourceType resourceType, float quantity)
    {
        ResourceType = resourceType;
        Quantity = quantity;
    }

    public bool Consume(Zone zone, float amount = 1)
    {
        if (Quantity <= 0)
        {
            zone.Resources.Remove(this);
            return false;
        }

        Quantity -= amount;
        return true;
    }


}
