﻿using Microsoft.Extensions.Configuration;
using static DataIntegrationTool.Shared.Utils.Constants;
using System.Text;
using DataIntegrationTool.Application.Config;
using DataIntegrationTool.Application.Models;
using DataIntegrationTool.Infrastructure.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DataIntegrationTool.Infrastructure.InputProviders;
using DataIntegrationTool.Application.Factories;
using static DataIntegrationTool.Shared.Utils.CsvEnums;
using DataIntegrationTool.Application.Interfaces;
using DataIntegrationTool.Application.DataCleaning;
using DataIntegrationTool.Infrastructure.DataAccess;


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


var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Configurazione
        services.AddSingleton(context.Configuration);

        // Reader e client
        services.AddTransient<IFileReader, FileReader>();
        services.AddHttpClient(); // HttpClient sarà risolto automaticamente dai provider
        services.AddScoped<IDatabaseAccessor>(sp =>
        {
            // Recupera la connection string (può essere vuota o null)
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connString = etlConfig.Input?.ConnectionString;

            // Passa la connection string anche se vuota
            return new DatabaseAccessor(connString);
        });

        // CsvService
        services.AddTransient<ICsvReaderService, CsvReaderService>();

        // Input Providers
        services.AddTransient<FileInputProvider>();
        services.AddTransient<HttpInputProvider>();
        services.AddTransient<StringInputProvider>();
        services.AddTransient<DatabaseInputProvider>();

        // Factory con i resolver
        services.AddSingleton(sp =>
        {
            return new InputProviderFactory(new Dictionary<InputType, InputProviderResolver>
            {
                [InputType.File] = config => sp.GetRequiredService<FileInputProvider>().WithConfig(config),
                [InputType.Http] = config => sp.GetRequiredService<HttpInputProvider>().WithConfig(config),
                [InputType.String] = config => sp.GetRequiredService<StringInputProvider>().WithConfig(config),
                [InputType.Database] = config => sp.GetRequiredService<DatabaseInputProvider>().WithConfig(config),
            });
        });
    })
    .Build();

var provider = builder.Services;

try
{
    var factory = provider.GetRequiredService<InputProviderFactory>();
    var csvService = new CsvReaderService();
    var inputProvider = factory.Create(etlConfig.Input!);
    var customers = await inputProvider.CreateObjectFromInputAsync<CustomerRaw>();

    Console.WriteLine("Customers loaded successfully:");

    var dataCleaner = new DefaultDataCleaner<CustomerRaw>();
    dataCleaner.Clean(customers);

    Console.WriteLine("Customers cleaned successfully:");

    foreach (var customer in customers)
    {
        Console.WriteLine($"{customer.FirstName} {customer.LastName} - {customer.Email}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"{ERRORMESSAGE}: {ex.Message}");
}