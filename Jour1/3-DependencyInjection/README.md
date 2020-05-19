# Injection de dépendances
.Net Core supporte l'injection de dépendance (design pattern) qui est une technique pour parvenir à l'inversion de contrôle entre les classes et leurs dépendances.

# Création de l'application
Créer une application de type ASP<i></i>.Net Core Web Application nommé `MyApplication`.

![alt text](newproject.jpg)

Sélectionner Empty comme type de projet et décocher HTTPS et Docker.
![alt text](projecttype.jpg)

# Survol de l'injection de dépendance
Une dépendance est un objet qu'un autre objet requiert.

```
public class MyDependency
{
    public MyDependency()
    {
    }

    public Task WriteMessage(string message)
    {
        Console.WriteLine(
            $"MyDependency.WriteMessage called. Message: {message}");

        return Task.FromResult(0);
    }
}
```

Une instance de MyDependency peut être créée pour rendre la méthode WriteMessage disponible à la classe:
```
public class IndexModel : PageModel
{
    MyDependency _dependency = new MyDependency();

    public async Task OnGetAsync()
    {
        await _dependency.WriteMessage(
            "IndexModel.OnGetAsync created this message.");
    }
}
```

La classe IndexModel crée et dépend directement de l'instance MyDependency. Ce type de dépendance est problématique et devrait être évité pour les raisons suivantes:
- Pour remplacer MyDependency avec une différente implémentation, la classe doit être modifiée.
- Si MyDependency a des dépendances, elles doivent être configurées par la classe. Dans une gros projet avec plusieurs classes qui dépendent sur MyDependency, la configuration devient éparpillée dans l'application.
- Cette implémentation est difficile à tester unitairement. L'application devrait utiliser un mock ou stubber la classe MyDependency, ce qui n'est pas possible avec cette approche.

L'injection de dépendance addresse ces problèmes grâce à :
- L'utilisation d'une interface ou d'une classe de base pour abstraire l'implémentation de la dépendance.
- L'enregistrement de la dépendance dans un service container.
- L'injection de service dans le constructeur d'une classe où elle est utilisée.

On obtient donc:
```
public interface IMyDependency
{
    Task WriteMessage(string message);
}
```

```
public class MyDependency : IMyDependency
{
    private readonly ILogger<MyDependency> _logger;

    public MyDependency(ILogger<MyDependency> logger)
    {
        _logger = logger;
    }

    public Task WriteMessage(string message)
    {
        _logger.LogInformation(
            "MyDependency.WriteMessage called. Message: {MESSAGE}", 
            message);

        return Task.FromResult(0);
    }
}
```

Pour l'ajouter au service container, il suffit d'ajouter la ligne suivante dans la méthode ConfigureServices de la classe Startup:
```
services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
```

# Durée de vie des services
Trois types de durée de vie sont disponibles:

## Transient
Un service de durée Transient est créé à chaque fois que ce dernier est sollicité du service container. C'est l'idéal pour les petits services qui sont stateless.

## Scoped
Un service de durée Scoped est créé une fois par requête du client (par connexion).

*** ATTENTION ***<BR>
Quand vous utilisez un service Scoped, il faut injecter le service par la méthode Invoke ou InvokeAsync. Ne pas injecter par le constructeur parce que cela force le service à agir comme un singleton.

## Singleton
Un service de durée Singleton est créé la première fois qu'il est sollicité. Toutes les utilisation subséquentes utilisent la même instance.

*** ATTENTION ***<BR>
Il est dangereux de résoudre un service Scoped dans un Singleton. Cela peut faire en sorte que le service aura un état incorrect lors des appels subséquents.

# Référence
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.1