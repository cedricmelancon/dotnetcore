# Routing
Le routing est responsable de faire le lien pour une requête HTTP et de transférer vers la partie de l'application responsable de l'exécution de la requête.

Les applications peuvent configurer les routes en utilisant:
- Controllers
- [Razor Pages](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/razor-pages-conventions?view=aspnetcore-3.1)
- SignalR
- gRPC Services
- Endpoint-enabled middleware (exemple Health Checks)
- Delegates et lambdas registered routines

# La base
Pour mettre en place les routes, il faut le faire au niveau de la fonction Configure du Startup.

```
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseRouting();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/", async context =>
        {
            await context.Response.WriteAsync("Hello World!");
        });
    });
}
```

Le `useRouting` ajoute l'association de routes au middleware pipeline. Ce middleware regarde tous les endpoint définis dans l'application et sélectionne la meilleure route pour la requête.

Le `useEndpoints` ajoute les endpoints au middleware pipeline. Il roule le delegate associé au endpoint sélectionné.

# Endpoint
Les méthodes Map### (MapGet, MapPost, etc.) sont utilisées pour définir un endpoint.

D'autres méthodes peuvent être appelées selon le type de route:
- MapRazorPages (pour Razor Pages)
- MapControllers (pour des controlleurs)
- MapHub\<THub> (pour SignalR)
- MapGrpcService\<TService> (pour gRPC)

Exemple de définition de Endpoints:
```
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    // Matches request to an endpoint.
    app.UseRouting();

    // Endpoint aware middleware. 
    // Middleware can use metadata from the matched endpoint.
    app.UseAuthentication();
    app.UseAuthorization();

    // Execute the matched endpoint.
    app.UseEndpoints(endpoints =>
    {
        // Configure the Health Check endpoint and require an authorized user.
        endpoints.MapHealthChecks("/healthz").RequireAuthorization();

        // Configure another endpoint, no authorization requirements.
        endpoints.MapGet("/", async context =>
        {
            await context.Response.WriteAsync("Hello World!");
        });

        endpoints.MapGet("/hello/{name:alpha}", async context =>
        {
            var name = context.Request.RouteValues["name"];
            await context.Response.WriteAsync($"Hello {name}!");
        }
    });
}
```
# Référence
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-3.1