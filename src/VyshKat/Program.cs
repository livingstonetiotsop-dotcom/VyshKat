using Microsoft.Extensions.DependencyInjection;
using VyshKat.Services;

var services = new ServiceCollection();
services.AddSingleton<IServiceCenter, ServiceCenter>();
services.AddSingleton<FleetService>();

using var provider = services.BuildServiceProvider();
var fleet = provider.GetRequiredService<FleetService>();

new ConsoleApp(fleet, Console.In, Console.Out).Run();
