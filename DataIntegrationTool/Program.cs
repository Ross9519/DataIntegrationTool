using DataIntegrationTool.Services.Interfaces;
using DataIntegrationTool.Services;
using DataIntegrationTool.Models;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var csvOptions = configuration.GetSection("CsvReaderOptions").Get<CsvReaderOptions>();
var inputSource = configuration.GetSection("InputSource").Get<InputSource>();

if (inputSource == null || string.IsNullOrWhiteSpace(inputSource.Type))
{
    throw new InvalidOperationException("InputSource configuration is missing or invalid.");
}

ICsvReaderService csvReader = new CsvReaderService();

try
{
    IEnumerable<CustomerRaw> customers;

    if (inputSource.Type == "File")
    {
        customers = await csvReader.HandleFilePathAsync<CustomerRaw>(inputSource.FilePath, csvOptions);
    }
    else if (inputSource.Type == "String")
    {
        customers = await csvReader.HandleContentAsync<CustomerRaw>(inputSource.Content, csvOptions);
    }
    else
    {
        throw new InvalidOperationException("Tipo InputSource non valido.");
    }

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