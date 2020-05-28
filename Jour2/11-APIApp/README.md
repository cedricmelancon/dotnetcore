# API App
Il est possible de créer des services RESTful. Pour supporter les requêtes, un web API utilise les controlleurs. Ces controlleurs dérivent de la classe `ControllerBase`.

# La classe ControllerBase
Pour définir un contrôlleur, il surffit d'utiliser des décorateurs et de dériver de la classe `ControllerBase` et non de la classe `Controller`. La classe `Controller` dérive de `ControllerBase` et ajoute le support des views, donc c'est pour les web pages et non les web API. Dans le cas où la classe sert autant à la vue que pour l'API, vous pouvez utiliser `Controller`, mais ce n'est pas recommandé.

```
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
```

La classe `ControllerBase` fourni plusieurs propriétés et méthodes utiles pour gérer les requêtes HTTP. Par exemple `ControllerBase.CreatedAtAction` retourne un status 201.

```
[HttpPost]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public ActionResult<Pet> Create(Pet pet)
{
    pet.Id = _petsInMemoryStore.Any() ? 
             _petsInMemoryStore.Max(p => p.Id) + 1 : 1;
    _petsInMemoryStore.Add(pet);

    return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
}
```

Voici d'autres méthodes:
- [BadRequest](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase.badrequest?view=aspnetcore-3.1)
- [NotFound](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase.notfound?view=aspnetcore-3.1)
- [PhysicalFile](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase.physicalfile?view=aspnetcore-3.1)
- [TryUpdateModelAsync](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase.tryupdatemodelasync?view=aspnetcore-3.1)
- [TryValidateModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase.tryvalidatemodel?view=aspnetcore-3.1)

# Attributs
Le framework fournit plusieurs attributs donc ceux-ci:
- [Route](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.routeattribute?view=aspnetcore-3.1)
- [Bind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.bindattribute?view=aspnetcore-3.1)
- [HttpGet](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.httpgetattribute?view=aspnetcore-3.1)
- [Consumes](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.consumesattribute?view=aspnetcore-3.1)
- [Produces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.producesattribute?view=aspnetcore-3.1)
Voici un exemple d'utilisation d'attributs:
```
[HttpPost]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public ActionResult<Pet> Create(Pet pet)
{
    pet.Id = _petsInMemoryStore.Any() ? 
             _petsInMemoryStore.Max(p => p.Id) + 1 : 1;
    _petsInMemoryStore.Add(pet);

    return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
}
```

L'attribut `ApiController` peut être utilisé sur une classe Controller pour permettre les comportements suivants:
- [Le requis de route](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.1#attribute-routing-requirement)
- [Réponse HTTP 400 automatique](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.1#automatic-http-400-responses)
- [Binding de la source des paramètres](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.1#binding-source-parameter-inference)
- [Multipart/Form-data](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.1#multipartform-data-request-inference)
- [Ajout du problem details lors d'un code d'erreur](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.1#problem-details-for-error-status-codes)

# Requis de route
L'attribut Route permet d'identifier le chemin pour lequel le controller doit répondre.

```
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
```

# Réponse HTTP 400 automatique
L'attribut ApiController fait la validation d'erreur automatiquement et génère une réponse HTTP 400. Par exemple, il n'est pas nécessaire d'écrire:

```
if (!ModelState.IsValid)
{
    return BadRequest(ModelState);
}
```

Il est possible de désactiver ce comportement de la façon suivante:
```
services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressConsumesConstraintForFormFileParameters = true;
        options.SuppressInferBindingSourcesForParameters = true;
        options.SuppressModelStateInvalidFilter = true;
        options.SuppressMapClientErrors = true;
        options.ClientErrorMapping[404].Link =
            "https://httpstatuses.com/404";
    });
```

# Binding de la source des paramètres
Il est possible de définir à l'aide d'un attribut la source d'un paramètre pour une requête.
- FromBody
- FromForm
- FromHeader
- FromQuery
- FromRoute
- FromServices

Sans l'utilisation de ApiController ou de l'attribut de binding (voir plus haut), ASP.NET Core tente d'utiliser un binder complexe de modélisation d'objet. Il est donc recommandé d'utiliser ces attributs.

```
[HttpGet]
public ActionResult<List<Product>> Get(
    [FromQuery] bool discontinuedOnly = false)
{
    List<Product> products = null;

    if (discontinuedOnly)
    {
        products = _productsInMemoryStore.Where(p => p.IsDiscontinued).ToList();
    }
    else
    {
        products = _productsInMemoryStore;
    }

    return products;
}
```

# Multipart/Form-data
Lorsque l'attribut FromForm est utilisé, le type de contenu multipart/form-data est utilisé. Il est possible de désactiver ce type de contenue de la façon suivante:
```
services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressConsumesConstraintForFormFileParameters = true;
        options.SuppressInferBindingSourcesForParameters = true;
        options.SuppressModelStateInvalidFilter = true;
        options.SuppressMapClientErrors = true;
        options.ClientErrorMapping[404].Link =
            "https://httpstatuses.com/404";
    });
```

# Ajout du problem details lors d'un code d'erreur
Lorsqu'on retourne un status code d'erreur, le framework ajoute automatiquement le détail du problème. Par exemple:
```
{
  type: "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  title: "Not Found",
  status: 404,
  traceId: "0HLHLV31KRN83:00000001"
}
```

Il est possible de désactiver ce comportement de la façon suivante:
```
services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressConsumesConstraintForFormFileParameters = true;
        options.SuppressInferBindingSourcesForParameters = true;
        options.SuppressModelStateInvalidFilter = true;
        options.SuppressMapClientErrors = true;
        options.ClientErrorMapping[404].Link =
            "https://httpstatuses.com/404";
    });
```

# Utilisation de l'attribut Consume
Il est possible de signifier le type de media qu'une requête accepte:
```
[HttpPost]
[Consumes("application/xml")]
public IActionResult CreateProduct(Product product)
```

Lorsque le type de contenu n'est pas le bon, le status code 415 Unsupported Media Type est retourné.
# Référence
https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.1