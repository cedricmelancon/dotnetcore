# HTTP Requests
Il est possible d'utiliser un `IHttpClientFactory` pour créer des instances de `HttpClient` dans l'application. 

# Usage de base
`IHttpClientFactory` peut être enregistré en appelant `AddHttpClient`.

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
        services.AddHttpClient();
        // Remaining code deleted for brevity.
```

Au niveau des services:
```
public class BasicUsageModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public IEnumerable<GitHubBranch> Branches { get; private set; }

    public bool GetBranchesError { get; private set; }

    public BasicUsageModel(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task OnGet()
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            "https://api.github.com/repos/aspnet/AspNetCore.Docs/branches");
        request.Headers.Add("Accept", "application/vnd.github.v3+json");
        request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            using var responseStream = await response.Content.ReadAsStreamAsync();
            Branches = await JsonSerializer.DeserializeAsync
                <IEnumerable<GitHubBranch>>(responseStream);
        }
        else
        {
            GetBranchesError = true;
            Branches = Array.Empty<GitHubBranch>();
        }
    }
}
```

# Clients nommés
Les clients nommés sont un bon choix quand:
- L'application requiert plusieurs `HttpClient` distincts.
- Plusieurs `HttpClient` avec différentes configurations.

```
services.AddHttpClient("github", c =>
{
    c.BaseAddress = new Uri("https://api.github.com/");
    // Github API versioning
    c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
    // Github requires a user-agent
    c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
});
```

Dans le service, nous avons:
```
public class NamedClientModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public IEnumerable<GitHubPullRequest> PullRequests { get; private set; }

    public bool GetPullRequestsError { get; private set; }

    public bool HasPullRequests => PullRequests.Any();

    public NamedClientModel(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task OnGet()
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            "repos/aspnet/AspNetCore.Docs/pulls");

        var client = _clientFactory.CreateClient("github");

        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            using var responseStream = await response.Content.ReadAsStreamAsync();
            PullRequests = await JsonSerializer.DeserializeAsync
                    <IEnumerable<GitHubPullRequest>>(responseStream);
        }
        else
        {
            GetPullRequestsError = true;
            PullRequests = Array.Empty<GitHubPullRequest>();
        }
    }
}
```

# Clients typés
- Fourni la même fonctionalité que le client nommé, mais sans utiliser une string comme clé.
- Fourni IntelliSense et l'aide du compilateur lorsqu'on consomme le client.
- Fourni un seul point pour la configuration et l'interaction avec un `HttpClient` en particulier.
- Fonctionne avec l'injection de dépendance.

Exemple:
```
public class GitHubService
{
    public HttpClient Client { get; }

    public GitHubService(HttpClient client)
    {
        client.BaseAddress = new Uri("https://api.github.com/");
        // GitHub API versioning
        client.DefaultRequestHeaders.Add("Accept",
            "application/vnd.github.v3+json");
        // GitHub requires a user-agent
        client.DefaultRequestHeaders.Add("User-Agent",
            "HttpClientFactory-Sample");

        Client = client;
    }

    public async Task<IEnumerable<GitHubIssue>> GetAspNetDocsIssues()
    {
        var response = await Client.GetAsync(
            "/repos/aspnet/AspNetCore.Docs/issues?state=open&sort=created&direction=desc");

        response.EnsureSuccessStatusCode();

        using var responseStream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync
            <IEnumerable<GitHubIssue>>(responseStream);
    }
}
```

Dans le Startup, nous avons:
```
services.AddHttpClient<GitHubService>();
```

Nous pouvons donc injecter le client de la façon suivante:
```
public class TypedClientModel : PageModel
{
    private readonly GitHubService _gitHubService;

    public IEnumerable<GitHubIssue> LatestIssues { get; private set; }

    public bool HasIssue => LatestIssues.Any();

    public bool GetIssuesError { get; private set; }

    public TypedClientModel(GitHubService gitHubService)
    {
        _gitHubService = gitHubService;
    }

    public async Task OnGet()
    {
        try
        {
            LatestIssues = await _gitHubService.GetAspNetDocsIssues();
        }
        catch(HttpRequestException)
        {
            GetIssuesError = true;
            LatestIssues = Array.Empty<GitHubIssue>();
        }
    }
}
```

La configuration d'un client typé peut être vue lors de l'enregistrement dans `Startup.ConfigurationServices`.
```
services.AddHttpClient<RepoService>(c =>
{
    c.BaseAddress = new Uri("https://api.github.com/");
    c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
    c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
});
```
# Handlers
Il est possible d'ajouter des handlers pour gérer les erreurs au niveau du client.
```
public void ConfigureServices(IServiceCollection services)
{           
    services.AddHttpClient<UnreliableEndpointCallerService>()
        .AddTransientHttpErrorPolicy(p => 
            p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

    // Remaining code deleted for brevity.
```

# Lifetime
Il est possible de gérer le temps de vie d'un client.

```
public void ConfigureServices(IServiceCollection services)
{           
    services.AddHttpClient("extendedhandlerlifetime")
        .SetHandlerLifetime(TimeSpan.FromMinutes(5));

    // Remaining code deleted for brevity.
```

Ceci permet de libérer la connection lorsqu'elle n'est pas utilisée et permet de détecter les changements de DNS. Par défaut, un lifetime de 2 minutes est utilisé.

# Logging
Les clients créée par `IHttpClientFactory` enregistrent les logs pour toutes les requêtes.
# Référence
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.1