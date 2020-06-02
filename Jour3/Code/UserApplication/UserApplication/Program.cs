using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace UserApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
                    if (hostContext.HostingEnvironment.IsDevelopment())
                    {
                        builder.AddUserSecrets<Program>();
                    }
                    else if (hostContext.HostingEnvironment.IsProduction())
                    {
                        var builtConfig = builder.Build();

                        var azureServiceTokenProvider = new AzureServiceTokenProvider();
                        var keyVaultClient = new KeyVaultClient(
                            new KeyVaultClient.AuthenticationCallback(
                                azureServiceTokenProvider.KeyVaultTokenCallback));

                        builder.AddAzureKeyVault(
                            $"https://{builtConfig["KeyVaultName"]}.vault.azure.net/",
                            keyVaultClient,
                            new DefaultKeyVaultSecretManager());
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
