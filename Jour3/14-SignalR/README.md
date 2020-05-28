# SignalR
SignalR est une librarie Open Source qui simplifie la fonctionnalité temps réel d'une application web.

Il est recommandé de l'utiliser dans les cas suivants:
- Application qui nécessite des mise-à-jour fréquentes du serveur (jeux, réseaux sociaux, etc.)
- Dashboard et applications de monitoring.
- Applications collaborative (whiteboard, team meeting)
- Applications qui nécessitent des notifications (email, chat, etc.)

Voici les fonctionnalités que SignalR propose:
- Gère automatiquement les connexions
- Envoie des messages à tous les clients connectés simultanément.
- Envoir des messages à un client spécifique ou un groupe de client.
- Scale pour absorber l'augmentation du traffic.

# Transports
SignalR supporte les techniques suivantes pour supporter les communications temps-réel:
- WebSockets
- Server-Sent Events
- Long Polling

# Hubs
SignalR utilise les Hubs pour communiquer entre les clients et les servers. Il s'agit d'un pipeline qui permet à un client et un serveur d'appeler des méthodes de un à l'autre.

# Configurer un hub SignalR

Pour utiliser SignalR, il faut tout d'abord ajouter les services:
```
services.AddSignalR();
```

Par la suite, il faut configurer le hub comme endpoint:
```
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chathub");
});
```

# Créer un Hub
Un hub est une classe qui dérive de la classe Hub:
```
public class ChatHub : Hub
{
    public Task SendMessage(string user, string message)
    {
        return Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
```

Pour envoyer des messages, trois façon sont disponibles:
- SendMessage: envoie un message à tous les clients en utilisant Clients.All
- SendMessageToCaller: envoie un message au client appelant en utilisant Clients.Caller
- SendMessageToGroups: envoie un message à tous les clients d'un groupe en utilisant Clients.Group.

```
public Task SendMessage(string user, string message)
{
    return Clients.All.SendAsync("ReceiveMessage", user, message);
}

public Task SendMessageToCaller(string message)
{
    return Clients.Caller.SendAsync("ReceiveMessage", message);
}

public Task SendMessageToGroup(string message)
{
    return Clients.Group("SignalR Users").SendAsync("ReceiveMessage", message);
}
```

L'inconvénient avec l'utilisation de SendAsync est qu'on doit utiliser un magic string (ReceiveMessage). Pour contrer ce problème, il est possible de créer un hub typé.

```
public interface IChatClient
{
    Task ReceiveMessage(string user, string message);
    Task ReceiveMessage(string message);
}
```

```
public class StronglyTypedChatHub : Hub<IChatClient>
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.ReceiveMessage(user, message);
    }

    public Task SendMessageToCaller(string message)
    {
        return Clients.Caller.ReceiveMessage(message);
    }
}
```

Il est possible de changer le nom d'une méthode dans un hub en utilisant les annotations:
```
[HubMethodName("SendMessageToUser")]
public Task DirectMessage(string user, string message)
{
    return Clients.User(user).SendAsync("ReceiveMessage", message);
}
```

# Obtenir le IHubContext
Le IHubContext peut être obtenu par injection de dépendances:
```
public class HomeController : Controller
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public HomeController(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task<IActionResult> Index()
    {
        await _hubContext.Clients.All.SendAsync("Notify", $"Home page loaded at: {DateTime.Now}");
        return View();
    }
}

```

# Users
Un utilisateur simple peut avoir plusieurs connexion à l'application (plusieurs browsers d'ouverts). Si un message est envoyé à un utilisateur, toutes les connexions vont recevoir le message.

Par défaut, SignalR utilise `ClaimTypes.NameIdentifier` du `ClaimsPrincipal` associé à la connexion comme identifiant de l'utilisateur. Il est possible de modifier ce comportement en utilisant des [claim personnalisés](https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-3.1#use-claims-to-customize-identity-handling).

```
public Task SendPrivateMessage(string user, string message)
{
    return Clients.User(user).SendAsync("ReceiveMessage", message);
}
```

# Groups
Un groupe est une collection de connexions associés à l'aide d'un nom. Les messages sont donc envoyés à toutes les connexions du groupe. Une connexion peut être membre de plusieurs groupes. Pour gérer les groupes:
```
public async Task AddToGroup(string groupName)
{
    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

    await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
}

public async Task RemoveFromGroup(string groupName)
{
    await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

    await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
}
```

Lorsqu'une connexion est fermée, son association au groupe est perdu. Il faut donc le gérer continuellement.

# Différences entre SignalR et SignalR Core
## Automatic Reconnects
L'automatic reconnect est maintenant Opt-In dans la version Core alors que c'était automatique avant.
```
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();
```
## Protocols supportés
La version Core support JSON ainsi que les nouveaux protocoles binaires basés sur [MessagePack](https://docs.microsoft.com/en-us/aspnet/core/signalr/messagepackhubprotocol?view=aspnetcore-3.1)

## Différences au niveau serveur
SignalR est maintenant inclus dans Microsoft.AspNetCore.App et doit être configuré de la façon que nous avons vu plus tôt.

### Single hub per connection
Dans la version Core, le modèle de connexion a été simplifié. Les connexions sont fait directement vers un hub unique au lieu de partager la connexion entre plusieurs hubs.

### Streaming
Le streaming de données est maintenant supporté entre un hub et les clients.

### États
Il n'est plus possible de passer un état (HubState) entre le hub et ses clients (pas de support pour les messages de progrès).

### PersistentConnection
Dans la version Core, le PersistentConnection a été supprimé.

### GlobalHost
Vu que c'est maintenant possible d'utiliser l'injection de dépendances pour obtenir le HubContext, l'objet GlobalHost a été supprimé.

### HubPipeline
Il n'y a pas de support pour le module de HubPipeline.

## Différences au niveau client
### Typescript
Le client est écrit en Typescript. L'utilisation de TypeScript et JavaScript est possible.

### NPM
Le client JavaScript est hosté sur NPM.

### jQuery
La dépendance à jQuery a été supprimée, mais les projets peuvent toujours l'utiliser.

### Support Internet Explorer
La version Core est supportée par la version 11+ d'Internet Explorer.

### JavaScript syntax
La syntaxe a changé. Au lieu d'utiliser l'objet $connection, il faut créer une connexion en utilisant l'API de HubConnectionBuilder.

```
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hub")
    .build();
```

### Hub Proxies
Les hub proxies ne sont plus générés automatiquement.

## Scaleout
La version Core supporte Azure SignalR Service et Redis alors qu'avant, c'était SQL Server et Redis.


# Référence
https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-3.1