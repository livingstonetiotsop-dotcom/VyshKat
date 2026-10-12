namespace VyshKat.Domain;

public sealed class Scooter : Vehicle, IEnergyConsumer
{
    public double DailyEnergyConsumptionKwh { get; }

    public Scooter(string inventoryNumber, int simplicity, bool isOperational, double dailyEnergyConsumptionKwh)
        : base("Electric scooter", inventoryNumber, simplicity, isOperational)
    {
        if (!double.IsFinite(dailyEnergyConsumptionKwh) || dailyEnergyConsumptionKwh < 0)
            throw new ArgumentOutOfRangeException(nameof(dailyEnergyConsumptionKwh), "Energy consumption must be a non-negative number.");
        DailyEnergyConsumptionKwh = dailyEnergyConsumptionKwh;
    }
}
