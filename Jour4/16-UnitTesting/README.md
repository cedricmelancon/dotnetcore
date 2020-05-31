# Unit Testing
Plusieurs framework sont disponibles pour faire des tests unitaires:
- MSTest
- xUnit
- NUnit

Voici un article qui compare les trois approches:
https://medium.com/@robinding/unit-testing-frameworks-xunit-vs-nunit-vs-mstest-for-net-and-net-core-e526011c0bd2

Microsoft a lui même choisi xUnit comme framework de test.
https://visualstudiomagazine.com/articles/2018/11/01/net-core-testing.aspx

# xUnit
Le framework xUnit s'installe à l'aide d'une librairie Nuget. Contrairement aux autres framework, xUnit ne se base pas sur un Setup et Test, mais plutôt sur des Fact et Theory.

Voici un exemple de test:
```
using Xunit;
using Prime.Services;

namespace Prime.UnitTests.Services
{
    public class PrimeService_IsPrimeShould
    {
        private readonly PrimeService _primeService;

        public PrimeService_IsPrimeShould()
        {
            _primeService = new PrimeService();
        }

        [Fact]
        public void IsPrime_InputIs1_ReturnFalse()
        {
            var result = _primeService.IsPrime(1);

            Assert.False(result, "1 should not be prime");
        }
    }
}
```

Le constructeur de la classe représente donc le setup.

L'attribut Fact déclare une méthode de test qui sera exécutée par le test runner.

Une autre façon de faire un test est avec l'attribut Theory:
```
[Theory]
[InlineData(-1)]
[InlineData(0)]
[InlineData(1)]
public void IsPrime_ValuesLessThan2_ReturnFalse(int value)
{
    var result = _primeService.IsPrime(value);

    Assert.False(result, $"{value} should not be prime");
}
```

L'attribut Theory représente une suite de tests à exécuter avec le même code, mais avec des valeurs d'entrée différentes. L'attribut InlineData sert à représenter ces valeurs.

# NUnit
Comme xUnit, NUnit est inclus dans un projet par une librairie Nuget. Son utilisation est plus conventionnelle.

```
using NUnit.Framework;
using Prime.Services;

namespace Prime.UnitTests.Services
{
    [TestFixture]
    public class PrimeService_IsPrimeShould
    {
        private PrimeService _primeService;

        [SetUp]
        public void SetUp()
        {
            _primeService = new PrimeService();
        }

        [Test]
        public void IsPrime_InputIs1_ReturnFalse()
        {
            var result = _primeService.IsPrime(1);

            Assert.IsFalse(result, "1 should not be prime");
        }
```

L'attribut TestFixture indique que la classe contient des tests unitaires. L'attribut Test indique que la méthode est un test et SetUp indique la méthode qui doit être exécutée avant tous les tests.

Il est possible de réutiliser un test avec d'autres valeurs d'entrée:
```
[TestCase(-1)]
[TestCase(0)]
[TestCase(1)]
public void IsPrime_ValuesLessThan2_ReturnFalse(int value)
{
    var result = _primeService.IsPrime(value);

    Assert.IsFalse(result, $"{value} should not be prime");
}
```

# MSTest
Finalement, regardons la syntaxe pour faire le même test avec MSTest:
```
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Services;

namespace Prime.UnitTests.Services
{
    [TestClass]
    public class PrimeService_IsPrimeShould
    {
        private readonly PrimeService _primeService;

        public PrimeService_IsPrimeShould()
        {
            _primeService = new PrimeService();
        }

        [TestMethod]
        public void IsPrime_InputIs1_ReturnFalse()
        {
            var result = _primeService.IsPrime(1);

            Assert.IsFalse(result, "1 should not be prime");
        }
    }
}
```

TestClass indique que la classe contient des tests unitaires. TestMethod indique que la méthode est un test.

Pour rouler un même tests avec plusieurs valeurs d'entrée, nous obtenons:
```
[DataTestMethod]
[DataRow(-1)]
[DataRow(0)]
[DataRow(1)]
public void IsPrime_ValuesLessThan2_ReturnFalse(int value)
{
    var result = _primeService.IsPrime(value);

    Assert.IsFalse(result, $"{value} should not be prime");
}
```

# Tests d'intégration avec ASP<i></i>.NET Core
Pour pouvoir faire des tests d'intégration, il faut créer un test fixture avec l'application web. Pour ce faire, nous utilisont le WebApplicationFactory:

```
public class BasicTests 
    : IClassFixture<WebApplicationFactory<RazorPagesProject.Startup>>
{
    private readonly WebApplicationFactory<RazorPagesProject.Startup> _factory;

    public BasicTests(WebApplicationFactory<RazorPagesProject.Startup> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Index")]
    [InlineData("/About")]
    [InlineData("/Privacy")]
    [InlineData("/Contact")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("text/html; charset=utf-8", 
            response.Content.Headers.ContentType.ToString());
    }
}
```

Il est aussi possible de personnaliser le WebApplicationFactory dans le cas où on ne veut pas se connecter directement sur la base de données par exemple, mais plutôt sur un InMemory database.

```
public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup: class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the app's ApplicationDbContext registration.
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add ApplicationDbContext using an in-memory database for testing.
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            // Build the service provider.
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database
            // context (ApplicationDbContext).
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                var logger = scopedServices
                    .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                // Ensure the database is created.
                db.Database.EnsureCreated();

                try
                {
                    // Seed the database with test data.
                    Utilities.InitializeDbForTests(db);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the " +
                        "database with test messages. Error: {Message}", ex.Message);
                }
            }
        });
    }
}
```

Par la suite, nous utilisons cette classe dans notre classe de test:

```
public class IndexPageTests : 
    IClassFixture<CustomWebApplicationFactory<RazorPagesProject.Startup>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<RazorPagesProject.Startup> 
        _factory;

    public IndexPageTests(
        CustomWebApplicationFactory<RazorPagesProject.Startup> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
    }

    [Fact]
    public async Task Post_DeleteAllMessagesHandler_ReturnsRedirectToRoot()
    {
        // Arrange
        var defaultPage = await _client.GetAsync("/");
        var content = await HtmlHelpers.GetDocumentAsync(defaultPage);

        //Act
        var response = await _client.SendAsync(
            (IHtmlFormElement)content.QuerySelector("form[id='messages']"),
            (IHtmlButtonElement)content.QuerySelector("button[id='deleteAllBtn']"));

        // Assert
        Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/", response.Headers.Location.OriginalString);
    }
}
```