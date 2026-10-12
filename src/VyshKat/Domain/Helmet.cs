namespace VyshKat.Domain;

public sealed class Helmet : IInventoryItem
{
    public string InventoryNumber { get; }
    public string Name => "Helmet";

    public Helmet(string inventoryNumber)
    {
        if (string.IsNullOrWhiteSpace(inventoryNumber))
            throw new ArgumentException("Inventory number cannot be empty.", nameof(inventoryNumber));
        InventoryNumber = inventoryNumber.Trim();
    }
}
