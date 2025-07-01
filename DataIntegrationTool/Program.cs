using DataIntegrationTool.Services.Interfaces;
using DataIntegrationTool.Services;
using DataIntegrationTool.Models;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string filePath = configuration["InputFiles:CustomerCsvPath"] ?? throw new Exception("Percorso CSV non trovato nella configurazione");


ICsvReaderService csvReader = new CsvReaderService();

try
{
    var customers = await csvReader.ReadFromFileAsync<CustomerRaw>(filePath);

    Console.WriteLine("Customers loaded successfully:");
    foreach (var customer in customers)
    {
        Console.WriteLine($"{customer.FirstName} {customer.LastName} - {customer.Email}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Errore durante la lettura del file CSV: {ex.Message}");
}