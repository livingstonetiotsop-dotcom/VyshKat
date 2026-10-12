namespace VyshKat.Domain;

public sealed class Bicycle : Vehicle
{
    public Bicycle(string inventoryNumber, int simplicity, bool isOperational)
        : base("Bicycle", inventoryNumber, simplicity, isOperational) { }
}
