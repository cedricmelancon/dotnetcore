# Secrets
Les secrets sont des valeurs de données sensibles qui ne doivent pas se retrouver dans le code.

En développement, les secrets sont conservés à part du code afin de s'assurer que ces derniers ne soient pas mis en source control. On les retrouve aux endroits suivants:

Windows:<BR>
%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json

Linux/macOS:<BR>
~/.microsoft/usersecrets/<user_secrets_id>/secrets.json

Le `user_secrets_id` est la valeur qui se retrouve dans le fichier .csproj.

```
<PropertyGroup>
  <TargetFramework>netcoreapp3.1</TargetFramework>
  <UserSecretsId>79a3edd0-2092-40a2-a04d-dcb46d5ca9ed</UserSecretsId>
</PropertyGroup>
```

Pour ajouter les secrets à un projet, il suffit d'entrer la commande suivante:
```
dotnet user-secrets init
```

Il est aussi possible de le faire directement dans Visual Studio en cliquant-droit sur le projet et sélectionnant 'Manage User Secrets'.

Les secrets sont sauvegardés dans une format Json.

```
{
  "Movies": {
    "ConnectionString": "Server=(localdb)\\mssqllocaldb;Database=Movie-1;Trusted_Connection=True;MultipleActiveResultSets=true",
    "ServiceApiKey": "12345"
  }
}
```

On peut aussi les ajouter par ligne de commande:

```
dotnet user-secrets set "Movies:ServiceApiKey" "12345" --project "C:\apps\WebApp1\src\WebApp1"
```

# Accéder aux secrets
Les secrets sont chargés dans la Configuration de l'application:
```
var host = new HostBuilder()
    .ConfigureAppConfiguration((hostContext, builder) =>
    {
        // Add other providers for JSON, etc.

        if (hostContext.HostingEnvironment.IsDevelopment())
        {
            builder.AddUserSecrets<Program>();
        }
    })
    .Build();
```

Ils sont alors accédés de la même manière que n'importe quel élément de la configuration.

# Les secrets en production
L'utilisation de Azure Key Vault est prévilégié.

```
// using Microsoft.Azure.KeyVault;
// using Microsoft.Azure.Services.AppAuthentication;
// using Microsoft.Extensions.Configuration.AzureKeyVault;

public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, config) =>
        {
            if (context.HostingEnvironment.IsProduction())
            {
                var builtConfig = config.Build();

                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(
                        azureServiceTokenProvider.KeyVaultTokenCallback));

                config.AddAzureKeyVault(
                    $"https://{builtConfig["KeyVaultName"]}.vault.azure.net/",
                    keyVaultClient,
                    new DefaultKeyVaultSecretManager());
            }
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```

Le `KeyVaultName` se retrouve dans le fichier appsettings.json.

# Références
https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-3.1