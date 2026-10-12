namespace VyshKat.Domain;

public abstract class Vehicle : IInventoryItem
{
    public string InventoryNumber { get; }
    public string Name { get; }
    public int Simplicity { get; }
    public bool IsOperational { get; }

    protected Vehicle(string name, string inventoryNumber, int simplicity, bool isOperational)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(inventoryNumber))
            throw new ArgumentException("Inventory number cannot be empty.", nameof(inventoryNumber));
        if (simplicity is < 1 or > 10)
            throw new ArgumentOutOfRangeException(nameof(simplicity), "Simplicity must be between 1 and 10.");

        Name = name.Trim();
        InventoryNumber = inventoryNumber.Trim();
        Simplicity = simplicity;
        IsOperational = isOperational;
    }
}
