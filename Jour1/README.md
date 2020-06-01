# Nouveautés de C# 9.0

# Init-only property
```
new Person
{
    FirsName = "John",
    LastName = "Doe"
}
```

Avant
```
public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```

Maintenant
```
public class Person
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
}
```

Vu que le `init accessor` ne peut être appelé que lors de l'initiation, il peut modifier les champs `readonly`.

# Records

Un record est un objet immuable et qui agit comme une valeur.

```
public data class Person
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
}
```

Ils ne sont pas fait pour changer dans le temps. Il faut plutôt créer un nouveau `record` représentant un nouvel état:

```
var otherPerson = person with { LastName = "Hanselman" };
```

Un record définit implicitement un copy constructor (`protected Person(Person original)`). L'utilisation du `with` fait en sorte que ce dernier est appelé puis applique les changement indiqués.

# Disparition de la classe Program
Le code suivant:
```
using System;
class Program
{
    static void Main()
    {
        Console.WriteLine("Hello World!");
    }
}
```

devient:
```
using System;

Console.WriteLine("Hello World!");
```

Tous types d'appels sont permet. Il est possible de faire des `await` et il est possible de retourner un code de status. La variable `args` est maintenant une variable magique.

# Pattern Matching
Avant:
```
public static decimal CalculateToll(object vehicle) =>
    vehicle switch
    {
       ...
       
        DeliveryTruck t when t.GrossWeightClass > 5000 => 10.00m + 5.00m,
        DeliveryTruck t when t.GrossWeightClass < 3000 => 10.00m - 2.00m,
        DeliveryTruck _ => 10.00m,

        _ => throw new ArgumentException("Not a known vehicle type", nameof(vehicle))
    };
```

Maintenant
```
DeliveryTruck => 10.00m,
```

```
DeliveryTruck t when t.GrossWeightClass switch
{
    > 5000 => 10.00m + 5.00m,
    < 3000 => 10.00m - 2.00m,
    _ => 10.00m,
},
```

Il est aussi possible d'utiliser des opérateurs logiques:
```
DeliveryTruck t when t.GrossWeightClass switch
{
    < 3000 => 10.00m - 2.00m,
    >= 3000 and <= 5000 => 10.00m,
    > 5000 => 10.00m + 5.00m,
},
```

# Not
Avant
```
if (!(e is Customer)) { ... }
```

Maintenant
```
if (e is not Customer) { ... }
```