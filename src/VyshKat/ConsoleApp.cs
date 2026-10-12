using System.Globalization;
using VyshKat.Domain;
using VyshKat.Services;

public sealed class ConsoleApp
{
    private readonly FleetService _fleet;
    private readonly TextReader _input;
    private readonly TextWriter _output;

    public ConsoleApp(FleetService fleet, TextReader input, TextWriter output)
    {
        _fleet = fleet ?? throw new ArgumentNullException(nameof(fleet));
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    public void Run()
    {
        _output.WriteLine("========================================");
        _output.WriteLine(" VyshKat — Fleet Management System");
        _output.WriteLine("========================================");

        bool running = true;
        while (running)
        {
            PrintMenu();
            _output.Write("Select an option: ");
            string? choice = _input.ReadLine()?.Trim();
            if (choice is null)
            {
                running = false;
                break;
            }

            try
            {
                switch (choice)
                {
                    case "1": AddVehicle(); break;
                    case "2": AddInventoryItem(); break;
                    case "3": ShowInventory(); break;
                    case "4": ShowBeginnerVehicles(); break;
                    case "5":
                        _output.WriteLine(
                            $"Total daily energy consumption: {_fleet.GetTotalDailyEnergyConsumptionKwh():0.##} kWh");
                        break;
                    case "6":
                        _output.WriteLine($"Vehicles: {_fleet.GetTotalVehicleCount()}");
                        _output.WriteLine($"Other inventory items: {_fleet.GetOtherItemCount()}");
                        _output.WriteLine($"Total inventory items: {_fleet.GetAllInventoryItems().Count()}");
                        break;
                    case "0": running = false; break;
                    default:
                        _output.WriteLine("Invalid option. Please enter a number from 0 to 6.");
                        break;
                }
            }
            catch (ArgumentException ex)
            {
                _output.WriteLine($"Invalid data: {ex.Message}");
            }
            catch (FormatException)
            {
                _output.WriteLine("Invalid input format. Please try again.");
            }

            _output.WriteLine();
        }

        _output.WriteLine("Goodbye!");
    }

    private void PrintMenu()
    {
        _output.WriteLine("1. Add a vehicle");
        _output.WriteLine("2. Add an inventory item");
        _output.WriteLine("3. Display all inventory items and inventory numbers");
        _output.WriteLine("4. Display vehicles suitable for beginners");
        _output.WriteLine("5. Display total daily energy consumption");
        _output.WriteLine("6. Display fleet and inventory counts");
        _output.WriteLine("0. Exit");
    }

    private void AddVehicle()
    {
        _output.WriteLine("\nVehicle type: 1. Electric scooter  2. Bicycle  3. Electric bicycle");
        _output.Write("Choose type: ");
        string type = ReadRequiredText();

        _output.Write("Inventory number: ");
        string number = ReadRequiredText();
        int simplicity = ReadInt("Simplicity (1-10): ", 1, 10);
        bool operational = ReadYesNo("Is the vehicle operational and ready for inspection? (y/n): ");

        Vehicle vehicle = type switch
        {
            "1" => new Scooter(number, simplicity, operational,
                ReadDouble("Daily energy consumption (kWh): ", 0, 10000)),
            "2" => new Bicycle(number, simplicity, operational),
            "3" => new ElectricBicycle(number, simplicity, operational,
                ReadDouble("Daily energy consumption (kWh): ", 0, 10000)),
            _ => throw new ArgumentException("Unknown vehicle type.")
        };

        _fleet.AddVehicle(vehicle, out string message);
        _output.WriteLine(message);
    }

    private void AddInventoryItem()
    {
        _output.WriteLine("\nItem type: 1. Helmet  2. Charging station");
        _output.Write("Choose type: ");
        string type = ReadRequiredText();

        _output.Write("Inventory number: ");
        string number = ReadRequiredText();

        IInventoryItem item = type switch
        {
            "1" => new Helmet(number),
            "2" => new ChargingStation(number,
                ReadDouble("Daily energy consumption (kWh): ", 0, 10000)),
            _ => throw new ArgumentException("Unknown inventory item type.")
        };

        _fleet.AddItem(item, out string message);
        _output.WriteLine(message);
    }

    private void ShowInventory()
    {
        var items = _fleet.GetAllInventoryItems().ToList();
        if (items.Count == 0)
        {
            _output.WriteLine("The inventory is empty.");
            return;
        }

        _output.WriteLine("\nAll inventory items:");
        foreach (IInventoryItem item in items.OrderBy(x => x.InventoryNumber))
            _output.WriteLine($"- {item.Name} | Inventory number: {item.InventoryNumber}");
    }

    private void ShowBeginnerVehicles()
    {
        var vehicles = _fleet.GetBeginnerFriendlyVehicles().ToList();
        if (vehicles.Count == 0)
        {
            _output.WriteLine("No beginner-friendly vehicles are currently available.");
            return;
        }

        _output.WriteLine("\nVehicles suitable for beginners (simplicity >= 6):");
        foreach (Vehicle vehicle in vehicles)
            _output.WriteLine(
                $"- {vehicle.Name} | {vehicle.InventoryNumber} | Simplicity: {vehicle.Simplicity}/10");
    }

    private string ReadRequiredText()
    {
        string? value = _input.ReadLine();
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("This field cannot be empty.");
        return value.Trim();
    }

    private int ReadInt(string prompt, int min, int max)
    {
        _output.Write(prompt);
        if (!int.TryParse(_input.ReadLine(), out int value) || value < min || value > max)
            throw new FormatException($"Enter a whole number between {min} and {max}.");
        return value;
    }

    private double ReadDouble(string prompt, double min, double max)
    {
        string? input = _input.ReadLine()?.Trim().Replace(',', '.');
        if (!double.TryParse(input, NumberStyles.Float,
                CultureInfo.InvariantCulture, out double value)
            || !double.IsFinite(value) || value < min || value > max)
            throw new FormatException($"Enter a number between {min} and {max}.");
        return value;
    }

    private bool ReadYesNo(string prompt)
    {
        _output.Write(prompt);
        return _input.ReadLine()?.Trim().ToLowerInvariant() switch
        {
            "y" or "yes" => true,
            "n" or "no" => false,
            _ => throw new FormatException("Enter y/yes or n/no.")
        };
    }
}
