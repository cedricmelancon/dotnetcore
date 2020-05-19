# Startup de l'application
La classe Startup configure les services de l'application. 

Voici un Startup typique:

```
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
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
}
```

La classe Startup est instanciée dans la classe Program de la façon suivante:
```
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
```

Les services suivants peuvent être injectés dans le constructeur du Startup:
- [IWebHostEnvironment](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.hosting.iwebhostenvironment?view=aspnetcore-3.1)
- [IHostEnvironment](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.ihostenvironment?view=dotnet-plat-ext-3.1)
- [IConfiguration](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfiguration?view=dotnet-plat-ext-3.1)

```
public class Startup
{
    private readonly IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        _env = env;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        if (_env.IsDevelopment())
        {
        }
        else
        {
        }
    }
}
```

# La méthode ConfigureServices
La méthode Configure Services est:
- Optionnelle.
- Appelée par le host AVANT la méthode `Configure`.
- où les options de configuration sont initialisés.

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(
            Configuration.GetConnectionString("DefaultConnection")));
    services.AddDefaultIdentity<IdentityUser>(
        options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>();

    services.AddRazorPages();
}
```

# La méthode Configure
La méthode Configure est utilisée pour spécifier comment l'application répond aux requêtes HTTP. Le Request Pipeline est configuré en ajoutant des composants Middleware au IApplicationBuilder.

Il est possible de configurer le pipeline pour les composants suivants:
- [Developer Exception Page](https://docs.microsoft.com/fr-ca/aspnet/core/fundamentals/error-handling?view=aspnetcore-3.1#developer-exception-page)
- [Exception Handler](https://docs.microsoft.com/fr-ca/aspnet/core/fundamentals/error-handling?view=aspnetcore-3.1#exception-handler-page)
- [HTTP Strict Transport Security (HSTS)](https://docs.microsoft.com/fr-ca/aspnet/core/security/enforcing-ssl?view=aspnetcore-3.1&tabs=visual-studio#http-strict-transport-security-protocol-hsts)
- [HTTPS redirection](https://docs.microsoft.com/fr-ca/aspnet/core/security/enforcing-ssl?view=aspnetcore-3.1&tabs=visual-studio)
- [Static files](https://docs.microsoft.com/fr-ca/aspnet/core/fundamentals/static-files?view=aspnetcore-3.1)
- ASP<i></i>.NET Core [MVC](https://docs.microsoft.com/fr-ca/aspnet/core/mvc/overview?view=aspnetcore-3.1) et [Razor Pages](https://docs.microsoft.com/fr-ca/aspnet/core/razor-pages/?view=aspnetcore-3.1&tabs=visual-studio)

```
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
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
# Référence
https://docs.microsoft.com/fr-ca/aspnet/core/fundamentals/startup?view=aspnetcore-3.1
