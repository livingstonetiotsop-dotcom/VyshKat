using VyshKat.Domain;
using VyshKat.Services;
using Xunit;

namespace VyshKat.Tests;

public sealed class FleetServiceTests
{
    [Fact]
    public void AddVehicle_AddsVehicle_WhenInspectionPasses()
    {
        var fleet = new FleetService(new AlwaysPassInspection());
        var scooter = new Scooter("SC-001", 7, true, 1.5);

        bool accepted = fleet.AddVehicle(scooter, out _);

        Assert.True(accepted);
        Assert.Single(fleet.Vehicles);
    }

    [Fact]
    public void AddVehicle_DoesNotAddVehicle_WhenInspectionFails()
    {
        var fleet = new FleetService(new AlwaysFailInspection());
        var scooter = new Scooter("SC-002", 7, true, 1.5);

        bool accepted = fleet.AddVehicle(scooter, out string message);

        Assert.False(accepted);
        Assert.Empty(fleet.Vehicles);
        Assert.Contains("failed technical inspection", message);
    }

    [Fact]
    public void AddVehicle_RejectsDuplicateInventoryNumber_CaseInsensitively()
    {
        var fleet = new FleetService(new AlwaysPassInspection());
        fleet.AddVehicle(new Scooter("SC-003", 7, true, 1), out _);

        bool accepted = fleet.AddItem(new Helmet("sc-003"), out string message);

        Assert.False(accepted);
        Assert.Contains("already in use", message);
    }

    [Fact]
    public void GetTotalEnergy_SumsEnergyConsumersAcrossVehiclesAndItems()
    {
        var fleet = new FleetService(new AlwaysPassInspection());
        fleet.AddVehicle(new Scooter("SC-004", 7, true, 1.5), out _);
        fleet.AddVehicle(new Bicycle("BI-001", 8, true), out _);
        fleet.AddItem(new ChargingStation("CH-001", 2.25), out _);
        fleet.AddItem(new Helmet("HE-001"), out _);

        Assert.Equal(3.75, fleet.GetTotalDailyEnergyConsumptionKwh(), 2);
    }

    [Fact]
    public void GetBeginnerFriendlyVehicles_ReturnsOnlyVehiclesWithSimplicityAtLeastSix()
    {
        var fleet = new FleetService(new AlwaysPassInspection());
        fleet.AddVehicle(new Scooter("SC-005", 6, true, 1), out _);
        fleet.AddVehicle(new Bicycle("BI-002", 5, true), out _);
        fleet.AddVehicle(new Bicycle("BI-003", 10, true), out _);

        Assert.Equal(new[] { "SC-005", "BI-003" },
            fleet.GetBeginnerFriendlyVehicles().Select(v => v.InventoryNumber));
    }

    [Fact]
    public void AddVehicle_DoesNotAcceptDuplicateVehicle()
    {
        var fleet = new FleetService(new AlwaysPassInspection());
        fleet.AddVehicle(new Bicycle("BI-004", 8, true), out _);

        bool accepted = fleet.AddVehicle(new Scooter("BI-004", 8, true, 1), out _);

        Assert.False(accepted);
        Assert.Single(fleet.Vehicles);
    }

    [Fact]
    public void AddItem_AddsItem_WhenInventoryNumberIsUnique()
    {
        var fleet = new FleetService(new AlwaysPassInspection());

        bool added = fleet.AddItem(new Helmet("HE-100"), out string message);

        Assert.True(added);
        Assert.Single(fleet.OtherItems);
        Assert.Contains("was added", message);
    }

    [Fact]
    public void GetAllInventoryItems_IncludesVehiclesAndOtherItems()
    {
        var fleet = new FleetService(new AlwaysPassInspection());
        fleet.AddVehicle(new Bicycle("BI-110", 8, true), out _);
        fleet.AddItem(new Helmet("HE-110"), out _);

        var items = fleet.GetAllInventoryItems().ToList();

        Assert.Equal(2, items.Count);
        Assert.Contains(items, item => item.InventoryNumber == "BI-110");
        Assert.Contains(items, item => item.InventoryNumber == "HE-110");
    }

    [Fact]
    public void GetCounts_ReturnsCorrectVehicleAndItemCounts()
    {
        var fleet = new FleetService(new AlwaysPassInspection());
        fleet.AddVehicle(new Bicycle("BI-111", 8, true), out _);
        fleet.AddVehicle(new Scooter("SC-111", 7, true, 1.0), out _);
        fleet.AddItem(new Helmet("HE-111"), out _);

        Assert.Equal(2, fleet.GetTotalVehicleCount());
        Assert.Equal(1, fleet.GetOtherItemCount());
    }

    [Fact]
    public void GetTotalEnergy_ReturnsZero_WhenNoEnergyConsumersExist()
    {
        var fleet = new FleetService(new AlwaysPassInspection());
        fleet.AddVehicle(new Bicycle("BI-112", 8, true), out _);
        fleet.AddItem(new Helmet("HE-112"), out _);

        Assert.Equal(0.0, fleet.GetTotalDailyEnergyConsumptionKwh());
    }

    [Fact]
    public void AddVehicle_Throws_WhenVehicleIsNull()
    {
        var fleet = new FleetService(new AlwaysPassInspection());

        Assert.Throws<ArgumentNullException>(
            () => fleet.AddVehicle(null!, out _));
    }

    [Fact]
    public void AddItem_Throws_WhenItemIsNull()
    {
        var fleet = new FleetService(new AlwaysPassInspection());

        Assert.Throws<ArgumentNullException>(
            () => fleet.AddItem(null!, out _));
    }

    [Fact]
    public void Constructor_Throws_WhenServiceCenterIsNull()
    {
        Assert.Throws<ArgumentNullException>(
            () => new FleetService(null!));
    }

    private sealed class AlwaysPassInspection : IServiceCenter
    {
        public bool Inspect(Vehicle vehicle) => true;
    }

    private sealed class AlwaysFailInspection : IServiceCenter
    {
        public bool Inspect(Vehicle vehicle) => false;
    }
}
