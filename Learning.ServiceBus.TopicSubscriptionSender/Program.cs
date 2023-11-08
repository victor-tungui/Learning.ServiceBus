using Microsoft.Extensions.Configuration;
using System.Reflection;

Console.WriteLine("Hello, World!");


var builderConfig = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", false, true)
	.AddJsonFile("appsettings.Development.json", true, true);

IConfiguration configuration = builderConfig.Build();

