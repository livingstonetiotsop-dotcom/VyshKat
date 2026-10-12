using VyshKat.Domain;

namespace VyshKat.Services;

public sealed class FleetService
{
    private readonly IServiceCenter _serviceCenter;
    private readonly List<Vehicle> _vehicles = new();
    private readonly List<IInventoryItem> _otherItems = new();

    public FleetService(IServiceCenter serviceCenter)
    {
        _serviceCenter = serviceCenter ?? throw new ArgumentNullException(nameof(serviceCenter));
    }

    public IReadOnlyList<Vehicle> Vehicles => _vehicles.AsReadOnly();
    public IReadOnlyList<IInventoryItem> OtherItems => _otherItems.AsReadOnly();

    public bool AddVehicle(Vehicle vehicle, out string message)
    {
        ArgumentNullException.ThrowIfNull(vehicle);

        if (InventoryNumberExists(vehicle.InventoryNumber))
        {
            message = $"Inventory number '{vehicle.InventoryNumber}' is already in use.";
            return false;
        }

        if (!_serviceCenter.Inspect(vehicle))
        {
            message = $"Vehicle '{vehicle.InventoryNumber}' failed technical inspection.";
            return false;
        }

        _vehicles.Add(vehicle);
        message = $"Vehicle '{vehicle.Name}' was accepted into the fleet.";
        return true;
    }

    public bool AddItem(IInventoryItem item, out string message)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (InventoryNumberExists(item.InventoryNumber))
        {
            message = $"Inventory number '{item.InventoryNumber}' is already in use.";
            return false;
        }

        _otherItems.Add(item);
        message = $"Item '{item.Name}' was added to the inventory.";
        return true;
    }

    public double GetTotalDailyEnergyConsumptionKwh()
    {
        return _vehicles.OfType<IEnergyConsumer>()
            .Sum(x => x.DailyEnergyConsumptionKwh)
            + _otherItems.OfType<IEnergyConsumer>()
            .Sum(x => x.DailyEnergyConsumptionKwh);
    }

    public IEnumerable<Vehicle> GetBeginnerFriendlyVehicles()
    {
        return _vehicles.Where(vehicle => vehicle.Simplicity >= 6);
    }

    public IEnumerable<IInventoryItem> GetAllInventoryItems()
    {
        return _vehicles.Cast<IInventoryItem>().Concat(_otherItems);
    }

    public int GetTotalVehicleCount() => _vehicles.Count;
    public int GetOtherItemCount() => _otherItems.Count;

    private bool InventoryNumberExists(string inventoryNumber)
    {
        return GetAllInventoryItems().Any(item =>
            string.Equals(item.InventoryNumber, inventoryNumber, StringComparison.OrdinalIgnoreCase));
    }
}
