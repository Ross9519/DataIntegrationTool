using DataIntegrationTool.Models;
using Microsoft.Extensions.Configuration;
using DataIntegrationTool.Config;
using static DataIntegrationTool.Utils.Constants;
using System.Text;
using DataIntegrationTool.Providers.Factories;

var configuration = new ConfigurationBuilder()
    .AddJsonFile(APPSETTINGS, optional: false, reloadOnChange: true)
    .Build();

var etlConfig = configuration.GetSection(ETL).Get<EtlConfiguration>() 
    ?? throw new InvalidOperationException($"{ETL} {ERRORMESSAGEPROGRAM}");

var errorMessageBuilder = new StringBuilder();

if (etlConfig.Input == null)
    errorMessageBuilder.AppendLine($"{INPUT} {ERRORMESSAGEPROGRAM}");
if (etlConfig.Output == null)
    errorMessageBuilder.AppendLine($"{OUTPUT} {ERRORMESSAGEPROGRAM}");
if (!etlConfig.Transformations.Any())
    errorMessageBuilder.AppendLine($"{TRANSFORMATION} {ERRORMESSAGEPROGRAM}");

var errorMessage = errorMessageBuilder.ToString();

if(!string.IsNullOrEmpty(errorMessage))
    throw new InvalidOperationException(errorMessage);


try
{
    var inputProvider = InputProviderFactory.Create(etlConfig.Input!);
    var customers = await inputProvider.CreateObjectFromInputAsync<CustomerRaw>();

    Console.WriteLine("Customers loaded successfully:");
    foreach (var customer in customers)
    {
        Console.WriteLine($"{customer.FirstName} {customer.LastName} - {customer.Email}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"{ERRORMESSAGE}: {ex.Message}");
}