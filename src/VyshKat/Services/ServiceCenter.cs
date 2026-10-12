using VyshKat.Domain;

namespace VyshKat.Services;

/// <summary>
/// Accepts vehicles only when they are operational. More inspection rules can be added here
/// without changing FleetService or the console interface.
/// </summary>
public sealed class ServiceCenter : IServiceCenter
{
    public bool Inspect(Vehicle vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        return vehicle.IsOperational;
    }
}
