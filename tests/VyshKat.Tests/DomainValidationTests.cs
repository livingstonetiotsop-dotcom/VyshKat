using VyshKat.Domain;
using Xunit;

namespace VyshKat.Tests;

public sealed class DomainValidationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Vehicle_RejectsSimplicityOutsideRange(int simplicity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Bicycle("BI-100", simplicity, true));
    }

    [Fact]
    public void Scooter_RejectsNegativeEnergyConsumption()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Scooter("SC-100", 7, true, -1));
    }

    [Fact]
    public void ServiceCenter_RejectsVehicleThatIsNotOperational()
    {
        var serviceCenter = new VyshKat.Services.ServiceCenter();
        var scooter = new Scooter("SC-101", 7, false, 1);

        Assert.False(serviceCenter.Inspect(scooter));
    }

    [Fact]
    public void ElectricBicycle_CreatesObject_WithValidEnergyConsumption()
    {
        var bicycle = new ElectricBicycle("EB-001", 8, true, 1.5);

        Assert.Equal("Electric bicycle", bicycle.Name);
        Assert.Equal("EB-001", bicycle.InventoryNumber);
        Assert.Equal(8, bicycle.Simplicity);
        Assert.Equal(1.5, bicycle.DailyEnergyConsumptionKwh);
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void ElectricBicycle_RejectsInvalidEnergyConsumption(double energy)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ElectricBicycle("EB-002", 8, true, energy));
    }
}
