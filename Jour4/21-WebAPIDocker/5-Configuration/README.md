# Configuration dans .NET Core
La configuration est réalisée en utilisant un ou plusieurs fournisseurs de configuration. Ces fournisseurs lisent les données dans une source sous forme key-value pairs. Les sources possibles sont les suivantes:
- Settings files, such as appsettings.json
- Environment variables
- Azure Key Vault
- Azure App Configuration
- Command-line arguments
- Custom providers, installed or created
- Directory files
- In-memory .NET objects

La configuration peut être injecté dans une classe.

```
public class Index2Model : PageModel
{
    private IConfigurationRoot ConfigRoot;

    public Index2Model(IConfiguration configRoot)
    {
        ConfigRoot = (IConfigurationRoot)configRoot;
    }

    public ContentResult OnGet()
    {           
        string str = "";
        foreach (var provider in ConfigRoot.Providers.ToList())
        {
            str += provider.ToString() + "\n";
        }

        return Content(str);
    }
}
```

Pour accéder à un élément de la configuration, on peut utiliser l'objet IConfiguration comme un dictionnaire.

```
public class TestModel : PageModel
{
    // requires using Microsoft.Extensions.Configuration;
    private readonly IConfiguration Configuration;

    public TestModel(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public ContentResult OnGet()
    {
        var myKeyValue = Configuration["MyKey"];
        var title = Configuration["Position:Title"];
        var name = Configuration["Position:Name"];
        var defaultLogLevel = Configuration["Logging:LogLevel:Default"];


        return Content($"MyKey value: {myKeyValue} \n" +
                       $"Title: {title} \n" +
                       $"Name: {name} \n" +
                       $"Default Log Level: {defaultLogLevel}");
    }
}
```

## appsettings.json
Le appsetting est un fichier Json qui défini les valeurs de différentes configuration de l'application.

```
{
  "Position": {
    "Title": "Editor",
    "Name": "Joe Smith"
  },
  "MyKey":  "My appsettings.json Value",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

Le `JsonConfigurationProvider` par défaut charge les configuration dans l'ordre suivant:
- appsettings.json
- appsettings.`Environnement`.json -> par exemple appsettings.Production.json.

Les variables du fichier d'environnement écrasent donc les variables préalablement chargées du fichier appsettings.json.

## Lier les valeurs de configuration à l'aide du patron Options
La méthode prévilégiée pour lire les valeurs de configuration est le patron Options. Par exemple, les valeurs suivantes:

```
"Position": {
    "Title": "Editor",
    "Name": "Joe Smith"
  }
```

Créer la classe suivante pour conserver les valeurs:
```
public class PositionOptions
{
    public string Title { get; set; }
    public string Name { get; set; }
}
```

Puis, pour lier les données:
```
public class Test22Model : PageModel
{
    private readonly IConfiguration Configuration;

    public Test22Model(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public ContentResult OnGet()
    {
        var positionOptions = new PositionOptions();
        Configuration.GetSection("Position").Bind(positionOptions);

        return Content($"Title: {positionOptions.Title} \n" +
                       $"Name: {positionOptions.Name}");
    }
}
```

Il est aussi possible d'injecter les options par la suite.

```
private T AddConfiguration<T>(IServiceCollection services, string key) where T : class, new()
{
    var options = new T();

    _configuration.Bind(key, options);
    services.AddSingleton(options);
    return options;
}
```

## Sécurité et secrets
Recommandations pour les données de configuration:
- Ne JAMAIS emmagasiner de mot de passes ou données sensibles en clair dans les fichiers de configuration. Les secrets peuvent être utilisés pour sauvegarder ces données en développement.
- Ne pas utiliser de secrets de la production dans un environnement de développement ou de test.
- Spécifier les secrets à l'extérieur du projet pour ne pas qu'ils soient accidentellement commité dans le repository de source control.

Par défaut, les secrets sont lus après les valeurs des appsettings.json et appsettings.Environnement.json. Ceci fait en sorte que les valeurs sont écrasées par celles des secrets.

## Variables d'environnement
Par défaut, les variables d'environnement sont lues après les secrets. Comme ça, elles ont préséance sur les autres valeurs.

Lorsqu'une valeur est hiérarchique (par exemple MyData:DataValue), il faut remplacer le ':' par '__'.

Il est possible de filtrer les variables d'environnement en utilisant un préfixe:
```
config.AddEnvironmentVariables(prefix: "MyCustomPrefix_");
```

## Command line
Les arguments du command line sont finalement lues. Elles viennent donc écraser les valeurs déjà existantes.

```
dotnet run MyKey="My key from command line" Position:Title=Cmd Position:Name=Cmd_Rick
```

## Données hiérarchiques

Pour obtenir une valeur dans la hiérarchie.

```
{
  "Position": {
    "Title": "Editor",
    "Name": "Joe Smith"
  },
  "MyKey":  "My appsettings.json Value",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

Il suffit d'utiliser la hiérarchie dans l'appel de la configuration:
```
var title = Configuration["Position:Title"];
```
# Référence
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1