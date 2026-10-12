# VyshKat — Fleet Management System

VyshKat is a C#/.NET 8 console application for managing a small scooter-sharing fleet and its inventory. The application supports adding vehicles and inventory items, performing technical inspections, calculating daily energy consumption, identifying vehicles suitable for beginners, and displaying inventory information.

The application uses object-oriented design, SOLID principles, dependency injection, and unit tests.

## Requirements

- .NET 8 SDK
- Internet access on the first run if NuGet packages need to be restored

## How to Run the Application

From the repository root, run:

```bash
dotnet run --project src/VyshKat/VyshKat.csproj
```

The application displays an interactive menu and accepts user input. It does not rely on hardcoded demonstration records.

**Note:** Inventory data is stored in memory and is reset when the application restarts.

## How to Run the Tests

Run the complete test suite with:

```bash
dotnet test tests/VyshKat.Tests/VyshKat.Tests.csproj
```

To build the application in Release mode, run:

```bash
dotnet build src/VyshKat/VyshKat.csproj -c Release
```

## Features

- Add electric scooters, bicycles, and electric bicycles interactively.
- Perform a technical inspection before accepting a vehicle into the fleet.
- Reject vehicles that fail the inspection.
- Prevent duplicate inventory numbers.
- Add helmets and charging stations to the inventory.
- Display all inventory items and their inventory numbers.
- Display vehicles suitable for beginners, defined as vehicles with a simplicity rating of at least 6.
- Calculate total daily energy consumption in kilowatt-hours (kWh).
- Display vehicle counts, other inventory item counts, and total inventory counts.
- Validate user input and handle invalid menu choices.

## Project Architecture

The project separates domain logic, fleet operations, and console interaction.

- **Domain:** Contains domain entities and abstractions, including `Vehicle`, `IInventoryItem`, `IEnergyConsumer`, and concrete vehicle and inventory item classes.
- **Services:** Contains `FleetService`, `IServiceCenter`, and `ServiceCenter`. These classes implement fleet operations and the technical inspection process.
- **Program.cs:** Configures dependency injection and starts the application.
- **ConsoleApp.cs:** Handles the interactive menu, user input, output, and input validation.
- **tests/VyshKat.Tests:** Contains unit tests for domain validation, fleet operations, inspection dependency substitution, and console interactions.

The console layer communicates with the fleet service rather than implementing fleet business rules itself.

## SOLID Principles

### 1. Single Responsibility Principle (SRP)

Each class has a focused responsibility.

- `FleetService` manages fleet and inventory operations.
- `ServiceCenter` determines whether a vehicle passes the technical inspection.
- Domain classes represent and validate domain objects.
- `ConsoleApp` handles interaction with the user.
- `Program.cs` configures dependencies and starts the application.

This separation makes the code easier to maintain, test, and modify.

### 2. Open/Closed Principle (OCP)

The fleet service calculates energy consumption through the `IEnergyConsumer` abstraction rather than checking each concrete class individually.

To add another energy-consuming inventory item, a new class can implement `IEnergyConsumer`. The existing energy calculation can then work with that item without requiring a new type-specific condition.

A new vehicle type can inherit from `Vehicle` and implement any relevant capability interfaces.

However, the interactive console menu must be updated to let users create the new type. This is a deliberate limitation at the presentation layer. A plugin system or more advanced registration mechanism would introduce unnecessary complexity for the current assignment.

### 3. Liskov Substitution Principle (LSP)

Concrete vehicle classes can be used wherever a `Vehicle` is expected. Inventory items can also be processed through the `IInventoryItem` abstraction.

The concrete classes preserve the domain constraints defined by their base classes and interfaces, allowing the fleet service to operate on them through shared abstractions.

### 4. Interface Segregation Principle (ISP)

The application separates inventory identity from energy consumption.

- `IInventoryItem` represents an item that belongs to the inventory.
- `IEnergyConsumer` represents an object that consumes energy.

A regular bicycle or helmet does not need to implement an energy-consumption interface when that capability is irrelevant to it.

### 5. Dependency Inversion Principle (DIP)

`FleetService` depends on the `IServiceCenter` abstraction rather than directly depending on the concrete `ServiceCenter` implementation.

The inspection service is provided through constructor injection. The application registers the implementation using `Microsoft.Extensions.DependencyInjection`.

In unit tests, fake implementations of `IServiceCenter` can replace the real inspection service. This allows fleet behavior to be tested independently of the real inspection implementation.

## Technical Inspection

Before a vehicle is accepted into the fleet, it is submitted to the inspection service.

The current `ServiceCenter` implementation accepts a vehicle when its `IsOperational` property is `true` and rejects it otherwise. The console asks the user to indicate whether the vehicle is operational and ready for inspection.

This is a deliberately simple inspection rule for the assignment. Additional inspection conditions can be implemented within the inspection service without requiring the fleet service to depend on those implementation details.

## Required Questions

### Which SOLID principle is applied most explicitly, and where?

The Dependency Inversion Principle (DIP) is demonstrated explicitly in `FleetService`. It depends on `IServiceCenter`, an abstraction, rather than directly depending on `ServiceCenter`.

The dependency is supplied through constructor injection. Unit tests demonstrate that alternative inspection implementations can be injected, allowing inspection outcomes to be controlled during testing.

The Single Responsibility Principle (SRP) is also demonstrated by separating console interaction, fleet operations, and technical inspection into different classes.

### Which principle is limited or almost violated, and why is this acceptable?

The Open/Closed Principle (OCP) is not fully achieved at the console boundary. Adding a new vehicle type requires an additional option in the interactive menu and corresponding object creation logic.

This limitation is acceptable for the current scope because the menu is the presentation layer, while the existing fleet operations and energy calculation rely on abstractions. A more extensible registration mechanism could be introduced if the application needed to support many dynamically added vehicle types, but it would add complexity that is not currently justified.

### What would change if a new vehicle type, employee, or spare-parts warehouse were added?

**New vehicle type:** A new vehicle class could inherit from `Vehicle`. If it consumes energy, it could implement `IEnergyConsumer`. The console menu would also need an option to create the new type. Existing fleet operations could continue to work through the shared abstractions, provided the new type satisfies their contracts.

**Employee:** An employee should be represented by a separate domain class with employee-specific properties and behavior. It should not implement `IInventoryItem` merely to fit the existing inventory model, because an employee is not an inventory asset in this assignment.

**Spare-parts warehouse:** A warehouse could be modeled as a separate domain concept with its own responsibilities and abstractions. Whether individual spare parts should implement `IInventoryItem` would depend on the inventory requirements. The warehouse itself should not be forced into the existing inventory-item abstraction unless that matches the domain model.

### Which test covers the inspection decision or beginner rule, and how is the dependency replaced?

The inspection dependency is tested using fake implementations of `IServiceCenter`.

- `AddVehicle_DoesNotAddVehicle_WhenInspectionFails` uses `AlwaysFailInspection` to verify that a vehicle is not added when the inspection fails.
- `AddVehicle_AddsVehicle_WhenInspectionPasses` uses `AlwaysPassInspection` to verify that a vehicle can be added when the inspection passes.
- `GetBeginnerFriendlyVehicles_ReturnsOnlyVehiclesWithSimplicityAtLeastSix` verifies the beginner-friendly vehicle rule.

The fake inspection implementations are injected into `FleetService` through its constructor. This isolates the tests from the real inspection implementation and makes the expected outcomes predictable.

## Design Decisions and Trade-offs

- **In-memory storage:** The application keeps its inventory in memory. Data persistence was not implemented because it was not required for the current scope.
- **Unique inventory numbers:** Inventory numbers are unique across vehicles and other inventory items, using case-insensitive comparison.
- **Energy calculation:** Daily energy consumption is calculated through the `IEnergyConsumer` abstraction, avoiding unnecessary checks for concrete types.
- **Separation of concerns:** Console interaction is separated from fleet business logic.
- **Testability:** Fleet behavior can be tested with substitute inspection services, and console interactions can be tested using in-memory input and output streams.
- **Avoiding overengineering:** The application uses a small set of domain classes, interfaces, and services appropriate for the assignment rather than introducing unnecessary architectural complexity.

## Suggested Manual Demonstration

The following sequence demonstrates the main application features:

1. Add a working electric scooter with inventory number `SC-001`, simplicity rating `8`, and daily energy consumption of `1.5 kWh`.
2. Add a bicycle with inventory number `BI-001` and simplicity rating `5`.
3. Attempt to add a non-operational vehicle and demonstrate that the inspection rejects it.
4. Add a helmet and a charging station with daily energy consumption of `2.25 kWh`.
5. Display all inventory items and their inventory numbers.
6. Display beginner-friendly vehicles. Only vehicles with a simplicity rating of at least `6` should appear.
7. Display total daily energy consumption. The expected total is `3.75 kWh` for the example data.
8. Display vehicle, other inventory item, and total inventory counts.
9. Exit the application.

## Test Results

During development, the following results were observed:

- **Unit tests:** 29 passed, 0 failed.
- **Line coverage:** 74.21%.
- **Branch coverage:** 61.53%.

These coverage values reflect the coverage report generated during development. Run the test suite again to verify the current state of the submitted code.
