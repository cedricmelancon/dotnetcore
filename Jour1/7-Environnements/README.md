# Environnements
Pour déterminer l'environnement, ASP.NET Core lit la variable d'environnement suivante:
- DOTNET_ENVIRONMENT
- ASPNETCORE_ENVIRONMENT quand ConfigureWebHostDefaults est appelé.

La valeur ASPNETCORE_ENVIRONMENT écrase la valeur de DOTNET_ENVIRONMENT.

`IHostEnvironment.EnvironmentName` peut être initialisé à n'importe quelle valeur, mais les valeurs suivantes sont fournies par le framework:
- Development
- Staging
- Production (par défaut si les variables d'environnement ne sont pas initialisées)

Pour savoir quel est l'environnement, voici un exemple:
```
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    if (env.IsProduction() || env.IsStaging() || env.IsEnvironment("Staging_2"))
    {
        app.UseExceptionHandler("/Error");
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapRazorPages();
    });
}
```

Pour gérer les environnements, il suffit de l'indiquer dans le fichier lauchSettings.json
```
{
  "iisSettings": {
    "windowsAuthentication": false, 
    "anonymousAuthentication": true, 
    "iisExpress": {
      "applicationUrl": "http://localhost:64645",
      "sslPort": 44366
    }
  },
  "profiles": {
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "IISX-Production": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Production"
      }
    },
    "IISX-Staging": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Staging",
        "ASPNETCORE_DETAILEDERRORS": "1",
        "ASPNETCORE_SHUTDOWNTIMEOUTSECONDS": "3"
      }
    },
    "EnvironmentsSample": {
      "commandName": "Project",
      "launchBrowser": true,
      "applicationUrl": "https://localhost:5001;http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "KestrelStaging": {
      "commandName": "Project",
      "launchBrowser": true,
      "applicationUrl": "https://localhost:5001;http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Staging"
      }
    }
  }
}
```

# Références
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-3.1