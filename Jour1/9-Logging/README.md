# Logging
Quand on appelle CreateDefaultBuilder, les providers de logging suivants sont ajoutés:
- Console
- Debug
- EventSource
- EventLog (Windows seulement)

Si on veut seulement un provider en particulier (par exemple, la console), voici ce qu'il faut faire:

```
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```

# Creation de log
L'exemple suivant montre comment utiliser le logger.

```
public class AboutModel : PageModel
{
    private readonly ILogger _logger;

    public AboutModel(ILogger<AboutModel> logger)
    {
        _logger = logger;
    }
    public string Message { get; set; }

    public void OnGet()
    {
        Message = $"About page visited at {DateTime.UtcNow.ToLongTimeString()}";
        _logger.LogInformation(Message);
    }
}
```

```
[HttpGet]
public IActionResult Test1(int id)
{
    var routeInfo = ControllerContext.ToCtxString(id);

    _logger.Log(LogLevel.Information, MyLogEvents.TestItem, routeInfo);
    _logger.LogInformation(MyLogEvents.TestItem, routeInfo);

    return ControllerContext.MyDisplayRouteInfo();
}
```
# Configuration
Il est possible de configurer l'environnement de logging à partir des fichiers appsettings. Voici un exemple:

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

Niveaux de log:
- Trace
- Debug
- Information
- Warning
- Error
- Critical
- None

On peut aussi contrôler le niveau de log par provider:
```
{
  "Logging": {
    "LogLevel": { // All providers, LogLevel applies to all the enabled providers.
      "Default": "Error", // Default logging, Error and higher.
      "Microsoft": "Warning" // All Microsoft* categories, Warning and higher.
    },
    "Debug": { // Debug provider.
      "LogLevel": {
        "Default": "Information", // Overrides preceding LogLevel:Default setting.
        "Microsoft.Hosting": "Trace " // Debug:Microsoft.Hosting category.
      },
      "EventSource": { // EventSource provider
        "LogLevel": {
          "Default": "Warning" // All categories of EventSource provider.
        }
      }
    }
  }
}
```

# Catégorie de log
Quand un objet de type `ILogger` est créé, une catégorie est spécifiée. Cette catégorie est incluse dans chaque message créé par l'instance. La catégorie est arbitraire, mais par convention, on utilise le nom de la classe.

```
public class PrivacyModel : PageModel
{
    private readonly ILogger<PrivacyModel> _logger;

    public PrivacyModel(ILogger<PrivacyModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation("GET Pages.PrivacyModel called.");
    }
}
```

Ou

```
public class ContactModel : PageModel
{
    private readonly ILogger _logger;

    public ContactModel(ILoggerFactory logger)
    {
        _logger = logger.CreateLogger("TodoApi.Pages.ContactModel");
    }

    public void OnGet()
    {
        _logger.LogInformation("GET Pages.ContactModel called.");
    }
```

# Les méthodes de log
Pour logger un message, il suffit d'appeler la méthode associée au niveau de log (Log\<Niveau>, par exemple: LogError).

```
[HttpGet("{id}")]
public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
{
    _logger.LogInformation(MyLogEvents.GetItem, "Getting item {Id}", id);

    var todoItem = await _context.TodoItems.FindAsync(id);

    if (todoItem == null)
    {
        _logger.LogWarning(MyLogEvents.GetItemNotFound, "Get({Id}) NOT FOUND", id);
        return NotFound();
    }

    return ItemToDTO(todoItem);
}
```

La variable `MyLogEvents.GetItem` représente le event ID afin d'identifier le type de message (aide pour les outils de monitoring).

```
public class MyLogEvents
{
    public const int GenerateItems = 1000;
    public const int ListItems     = 1001;
    public const int GetItem       = 1002;
    public const int InsertItem    = 1003;
    public const int UpdateItem    = 1004;
    public const int DeleteItem    = 1005;

    public const int TestItem      = 3000;

    public const int GetItemNotFound    = 4000;
    public const int UpdateItemNotFound = 4001;
}
```

Pour logger une exception:
```
[HttpGet("{id}")]
public IActionResult TestExp(int id)
{
    var routeInfo = ControllerContext.ToCtxString(id);
    _logger.LogInformation(MyLogEvents.TestItem, routeInfo);

    try
    {
        if (id == 3)
        {
            throw new Exception("Test expception");
        }
    }
    catch (Exception ex)
    {
        _logger.LogWarning(MyLogEvents.GetItemNotFound, ex, "TestExp({Id})", id);
        return NotFound();
    }

    return ControllerContext.MyDisplayRouteInfo();
}
```

# Filtres
Il est possible d'ajouter des filtres à l'engin de loggin afin de restraindre certains logs.

```
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.AddFilter((provider, category, logLevel) =>
                {
                    if (provider.Contains("ConsoleLoggerProvider")
                        && category.Contains("Controller")
                        && logLevel >= LogLevel.Information)
                    {
                        return true;
                    }
                    else if (provider.Contains("ConsoleLoggerProvider")
                        && category.Contains("Microsoft")
                        && logLevel >= LogLevel.Information)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
```

# Scopes
Il est possible d'ajouter le scope au log.

```
{
  "Logging": {
    "Debug": {
      "LogLevel": {
        "Default": "Information"
      }
    },
    "Console": {
      "IncludeScopes": true, // Required to use Scopes.
      "LogLevel": {
        "Microsoft": "Warning",
        "Default": "Information"
      }
    },
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

```
[HttpGet("{id}")]
public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
{
    TodoItem todoItem;

    using (_logger.BeginScope("using block message"))
    {
        _logger.LogInformation(MyLogEvents.GetItem, "Getting item {Id}", id);

        todoItem = await _context.TodoItems.FindAsync(id);

        if (todoItem == null)
        {
            _logger.LogWarning(MyLogEvents.GetItemNotFound, 
                "Get({Id}) NOT FOUND", id);
            return NotFound();
        }
    }

    return ItemToDTO(todoItem);
}
```

# Azure App Service
Il est possible d'ajouter des logs dans un blob storage.

```
public class Scopes
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging => logging.AddAzureWebAppDiagnostics())
                .ConfigureServices(serviceCollection => serviceCollection
                    .Configure<AzureFileLoggerOptions>(options =>
                    {
                        options.FileName = "azure-diagnostics-";
                        options.FileSizeLimit = 50 * 1024;
                        options.RetainedFileCountLimit = 5;
                    })
                    .Configure<AzureBlobLoggerOptions>(options =>
                    {
                        options.BlobName = "log.txt";
                    }))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
```

Par défaut, le blob name sera: `{app-name}{timestamp}/yyyy/mm/dd/hh/{guid}-applicationLog.txt`

# Custom Logger
Il est possible de créer un logger personnalisé.

```
public class ColoredConsoleLogger : ILogger
{
    private readonly string _name;
    private readonly ColoredConsoleLoggerConfiguration _config;

    public ColoredConsoleLogger(string name, ColoredConsoleLoggerConfiguration config)
    {
        _name = name;
        _config = config;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel == _config.LogLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, 
                        Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        if (_config.EventId == 0 || _config.EventId == eventId.Id)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = _config.Color;
            Console.WriteLine($"{logLevel} - {eventId.Id} " +
                              $"- {_name} - {formatter(state, exception)}");
            Console.ForegroundColor = color;
        }
    }
}
```

Par la suite, on peut créer un custom LoggerProvider

```
public class ColoredConsoleLoggerProvider : ILoggerProvider
{
    private readonly ColoredConsoleLoggerConfiguration _config;
    private readonly ConcurrentDictionary<string, ColoredConsoleLogger> _loggers = new ConcurrentDictionary<string, ColoredConsoleLogger>();

    public ColoredConsoleLoggerProvider(ColoredConsoleLoggerConfiguration config)
    {
        _config = config;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new ColoredConsoleLogger(name, _config));
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
}
```

Pour l'enregistrer:

```
public void Configure(IApplicationBuilder app, IWebHostEnvironment env, 
                      ILoggerFactory loggerFactory)
{
    // Default registration.
    loggerFactory.AddProvider(new ColoredConsoleLoggerProvider(
                              new ColoredConsoleLoggerConfiguration
    {
        LogLevel = LogLevel.Error,
        Color = ConsoleColor.Red
    }));

    // Custom registration with default values.
    loggerFactory.AddColoredConsoleLogger();

    // Custom registration with a new configuration instance.
    loggerFactory.AddColoredConsoleLogger(new ColoredConsoleLoggerConfiguration
    {
        LogLevel = LogLevel.Debug,
        Color = ConsoleColor.Gray
    });

    // Custom registration with a configuration object.
    loggerFactory.AddColoredConsoleLogger(c =>
    {
        c.LogLevel = LogLevel.Information;
        c.Color = ConsoleColor.Blue;
    });

    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }
    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    });
}
```

Pour que le code précédent fonctionne, il faut ajouter une extension au `ILoggerFactory`.

```
public static class ColoredConsoleLoggerExtensions
{
    public static ILoggerFactory AddColoredConsoleLogger(
                                      this ILoggerFactory loggerFactory, 
                                      ColoredConsoleLoggerConfiguration config)
    {
        loggerFactory.AddProvider(new ColoredConsoleLoggerProvider(config));
        return loggerFactory;
    }
    public static ILoggerFactory AddColoredConsoleLogger(
                                      this ILoggerFactory loggerFactory)
    {
        var config = new ColoredConsoleLoggerConfiguration();
        return loggerFactory.AddColoredConsoleLogger(config);
    }
    public static ILoggerFactory AddColoredConsoleLogger(
                                    this ILoggerFactory loggerFactory, 
                                    Action<ColoredConsoleLoggerConfiguration> configure)
    {
        var config = new ColoredConsoleLoggerConfiguration();
        configure(config);
        return loggerFactory.AddColoredConsoleLogger(config);
    }
}
```
# Référence
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1