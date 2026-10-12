namespace VyshKat.Domain;

public sealed class ChargingStation : IInventoryItem, IEnergyConsumer
{
    public string InventoryNumber { get; }
    public string Name => "Charging station";
    public double DailyEnergyConsumptionKwh { get; }

    public ChargingStation(string inventoryNumber, double dailyEnergyConsumptionKwh)
    {
        if (string.IsNullOrWhiteSpace(inventoryNumber))
            throw new ArgumentException("Inventory number cannot be empty.", nameof(inventoryNumber));
        if (!double.IsFinite(dailyEnergyConsumptionKwh) || dailyEnergyConsumptionKwh < 0)
            throw new ArgumentOutOfRangeException(nameof(dailyEnergyConsumptionKwh), "Energy consumption must be a non-negative number.");

        InventoryNumber = inventoryNumber.Trim();
        DailyEnergyConsumptionKwh = dailyEnergyConsumptionKwh;
    }
}
