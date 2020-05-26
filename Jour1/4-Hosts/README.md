Nous allons couvrir deux types de Host:
- Generic Host
- Web Host

Un Host est un objet qui encapsule les ressources d'une application telles que:
- Injection de dépendances
- Logging
- Configuration
- Implémentations de IHostedService

Le host est typiquement configuré, construit et exécuté dans la classe `Program`. La méthode `Main`:
- Appelle la méthode `CreateHostBuilder` pour créer et configurer le constructeur d'objet.
- Appelle les méthodes `Build` et `Run` pour démarrer le host.

# Generic Host
Le Generic Host est utilisé pour des applications qui ne requiert pas le Web. Un Startup peut être utilisé ou non.

Sans Startup:
```
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
               services.AddHostedService<Worker>();
            });
}
```

Avec Startup:
```
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```

La méthode `CreateDefaultBuilder`:
- Initialise le root au chemin du `GetCurrentDirectory`
- Charge la configuration du host à partir:
  - Des variables d'environnement préfixés avec DOTNET_.
  - Des arguments de la ligne de commande
- Charge la configuration de l'application à partir de:
  - appsettings.json
  - appsettings.{Environnement}.json
  - secrets
  - variables d'environnement
  - arguments de la ligne de commande
- Ajoute les providers de logging suivants:
  - Console
  - Debug
  - EventSource
  - EventLog (seulement sous Windows)
- Permet la validation de la portée et des dépendances quand l'environnement est Development.

La méthode `ConfigureWebHostDefaults`:
- Charge la configuration du host à partir des variables d'environnement préfixées avec ASPNETCORE_.
- Défini le serveur Kestrel comme web server et le configure en utilisant les fournisseurs de configuration de l'application.
- Ajoute le middleware de filtrage du host
- Ajoute le middleware pour les entêtes transmis si ASPNETCORE_FORWARDEDHEADERS_ENABLED est à TRUE
- Permet l'intégration à IIS.

## IHostLifetime
L'implémentation de `IHostLifetime` contrôle lorsque le host démarre et arrête. Par défaut l'implémentation de IHostLifetime est Microsoft.Extensions.Hosting.Internal.ConsoleLifetime est utilisée. Il écoute pour le Ctrl-C/SIGINT ou SIGTERM et appelle StopApplication.

## IHostEnvironment
Injecte le service `IHostEnvironment` dans une classe afin d'obtenir les paramètres suivants:
- ApplicationName
- EnvironmentName
- ContentRootPath

`IWebHostEnvironment` ajoute le WebRootPath.

## Host Configuration
Pour ajouter la configuration du host, il suffit d'appeler `ConfigureHostConfiguration` sur `IHostBuilder`.

```
// using Microsoft.Extensions.Configuration;

Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(configHost =>
    {
        configHost.SetBasePath(Directory.GetCurrentDirectory());
        configHost.AddJsonFile("hostsettings.json", optional: true);
        configHost.AddEnvironmentVariables(prefix: "PREFIX_");
        configHost.AddCommandLine(args);
    });
```

## Configuration de l'application
Nous allons voir la configuration de l'application dans le module 5 (Configuration).

# Web Host
La méthode `CreateDefaultBuilder`:
- Configure le serveur Kestrel en tant que web server en utilisant les fournisseurs de configuration de l'application.
- Initialise le root au chemin du `GetCurrentDirectory`
- Charge la configuration du host à partir:
  - Des variables d'environnement préfixés avec ASPNETCORE_.
  - Des arguments de la ligne de commande
- Charge la configuration de l'application à partir de:
  - appsettings.json
  - appsettings.{Environnement}.json
  - secrets
  - variables d'environnement
  - arguments de la ligne de commande
- Ajoute les providers de logging suivants:
  - Console
  - Debug
- Lorsque roulée derrière IIS, `CreateDefaultBuilder` permet l'intégration IIS, qui configure l'adresse de base ainsi que le port de l'application.
- Permet la validation de la portée et des dépendances quand l'environnement est Development.

# Références
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-3.1