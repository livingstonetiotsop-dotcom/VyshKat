namespace VyshKat.Domain;

public sealed class ElectricBicycle : Vehicle, IEnergyConsumer
{
    public double DailyEnergyConsumptionKwh { get; }

    public ElectricBicycle(string inventoryNumber, int simplicity, bool isOperational, double dailyEnergyConsumptionKwh)
        : base("Electric bicycle", inventoryNumber, simplicity, isOperational)
    {
        if (!double.IsFinite(dailyEnergyConsumptionKwh) || dailyEnergyConsumptionKwh < 0)
            throw new ArgumentOutOfRangeException(nameof(dailyEnergyConsumptionKwh), "Energy consumption must be a non-negative number.");
        DailyEnergyConsumptionKwh = dailyEnergyConsumptionKwh;
    }
}
