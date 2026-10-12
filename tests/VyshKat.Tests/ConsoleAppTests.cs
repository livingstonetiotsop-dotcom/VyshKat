using VyshKat.Domain;
using VyshKat.Services;
using Xunit;

namespace VyshKat.Tests;

public sealed class ConsoleAppTests
{
    private static string RunApp(string input, FleetService? fleet = null)
    {
        var reader = new StringReader(input);
        var writer = new StringWriter();
        fleet ??= new FleetService(new ServiceCenter());

        new ConsoleApp(fleet, reader, writer).Run();
        return writer.ToString();
    }

    [Fact]
    public void Run_ExitsWhenUserChoosesZero()
    {
        string output = RunApp("0\n");

        Assert.Contains("Goodbye!", output);
    }

    [Fact]
    public void Run_ShowsErrorForInvalidMenuOption()
    {
        string output = RunApp("x\n0\n");

        Assert.Contains("Invalid option", output);
        Assert.Contains("Goodbye!", output);
    }

    [Fact]
    public void Run_AddsBicycleAndDisplaysItsInventoryNumber()
    {
        string output = RunApp("1\n2\nBI-TEST\n8\ny\n3\n0\n");

        Assert.Contains("BI-TEST", output);
        Assert.Contains("was accepted into the fleet", output);
        Assert.Contains("All inventory items:", output);
    }

    [Fact]
    public void Run_RejectsUnknownVehicleType()
    {
        string output = RunApp("1\n9\nBI-TEST\n8\ny\n0\n");

        Assert.Contains("Invalid data: Unknown vehicle type.", output);
    }

    [Fact]
    public void Run_DisplaysEmptyInventoryMessage()
    {
        string output = RunApp("3\n0\n");

        Assert.Contains("The inventory is empty.", output);
    }

    [Fact]
    public void Run_DisplaysZeroEnergyConsumption()
    {
        string output = RunApp("5\n0\n");

        Assert.Contains("Total daily energy consumption: 0 kWh", output);
    }

    [Fact]
    public void Run_RejectsVehicleThatFailsInspection()
    {
        string output = RunApp("1\n2\nBI-FAIL\n8\nn\n3\n0\n");

        Assert.Contains("failed technical inspection", output);
        Assert.Contains("The inventory is empty.", output);
    }
}
