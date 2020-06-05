# Action Return Type
ASP<i></i>.NET Core offre les options suivantes de type de retour:
- Type Spécifique
- IActionResult
- ActionResult\<T>

# Type spécifique
Les actions les plus simples peuvent retourner un type primitif ou complexe.
```
[HttpGet]
public List<Product> Get() =>
    _repository.GetProducts();
```

Il faut prendre en considération qu'aucune validation n'est faite s'il y a des paramètres. Dans le cas où on a besoin de mettre des contraintes, l'utilisation des autres types est à privilégier.

À partir de .Net Core 3.0, il est possible d'utiliser le type IAsyncEnumerable\<T>.

Donc au lieu d'avoir:
```
[HttpGet("syncsale")]
public IEnumerable<Product> GetOnSaleProducts()
{
    var products = _repository.GetProducts();

    foreach (var product in products)
    {
        if (product.IsOnSale)
        {
            yield return product;
        }
    }
}
```

on obtient:

```
[HttpGet("asyncsale")]
public async IAsyncEnumerable<Product> GetOnSaleProductsAsync()
{
    var products = _repository.GetProductsAsync();

    await foreach (var product in products)
    {
        if (product.IsOnSale)
        {
            yield return product;
        }
    }
}
```

Les deux appels sont non bloquants.

# IActionResult
Ce type de retour est approprié lorsque plusieurs types d'`ActionResult` sont possibles pour une action. Par exemple, une action pourrait retourner un status 400 (BadRequestResult), 404 (NotFoundResult) et 200 (OkObjectResult).

Version synchrone:
```
[HttpGet("{id}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public IActionResult GetById(int id)
{
    if (!_repository.TryGetProduct(id, out var product))
    {
        return NotFound();
    }

    return Ok(product);
}
```

Version asynchrone:
```
[HttpPost]
[Consumes(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> CreateAsync(Product product)
{
    if (product.Description.Contains("XYZ Widget"))
    {
        return BadRequest();
    }

    await _repository.AddProductAsync(product);

    return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
}
```

# ActionResult\<T>
À partir de .NET Core 2.1, ActionResult\<T> a été ajouté comme type de retour. C'est une combinaison du type spécifique et IActionResult. 

Il permet de négliger l'utilisation de type pour l'attribut `ProducesResponseType`. Il permet aussi un cast implicite pour la conversion de `T` et `ActionResult` en `ActionResult\<T>`.

Version Synchrone:
```
[HttpGet("{id}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public ActionResult<Product> GetById(int id)
{
    if (!_repository.TryGetProduct(id, out var product))
    {
        return NotFound();
    }

    return product;
}
```

Version asynchrone:
```
[HttpPost]
[Consumes(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<ActionResult<Product>> CreateAsync(Product product)
{
    if (product.Description.Contains("XYZ Widget"))
    {
        return BadRequest();
    }

    await _repository.AddProductAsync(product);

    return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
}
```

# Référence
https://docs.microsoft.com/en-us/aspnet/core/web-api/action-return-types?view=aspnetcore-3.1