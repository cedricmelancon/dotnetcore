# gRPC Services
gRPC est un framework de Remote Procedure Call très performant.

Les principaux bénéfices de gRPC sont:
- Framework RPC moderne, petit et très performant.
- Utilise Protocol Buffer pour la définition de l'API, ce qui le rend langage agnostique.
- Supporte client, serveur et appels de streaming bi-directionnel
- Réduit l'utilisation du réseau avec l'utilisation de la sérialisation binaire Protobuf.

Son utilisation est donc idéale pour:
- Petits micro-services où l'efficacité est critique.
- Système multi-langages
- Services Point-to-point temps réel qui nécessite de supporter des requêtes ou réponse en streaming.

# Fichiers proto
Pour définir l'API, il suffit de définir le contenu du message dans un fichier .proto.

```
syntax = "proto3";

service Greeter {
  rpc SayHello (HelloRequest) returns (HelloReply);
}

message HelloRequest {
  string name = 1;
}

message HelloReply {
  string message = 1;
}
```

Il est possible d'intégrer directement le générateur de code dans l'environnement .Net Core.

Il suffit d'utiliser le package nuget `Grpc.Tools` et de modifier le projet pour ajouter les fichiers .proto.

```
<ItemGroup>
  <Protobuf Include="Protos\greet.proto" />
</ItemGroup>
```

Il faudra installer les librairies suivantes pour réussir à compiler:
- Google.Protobuf
- Grpc.Core

# Enregistrement de gRPC
Deux étapes sont nécessaires pour utiliser gRPC. Premièrement, il faut ajouter le service gRPC:
```
services.AddGrpc();
```

Par la suite, il faut enregistrer le endpoint:
```
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapGrpcService<GreeterService>();
});
```

# Appels gRPC
Pour générer un appel, il faut se connecter au serveur et lui envoyer la requête.

```
// The port number(5001) must match the port of the gRPC server.
using var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = new Greeter.GreeterClient(channel);
var reply = client.SayHello(new HelloRequest { Name = "<Your Name Here>" });
_logger.LogInformation(reply.Message);
```

# Référence
https://docs.microsoft.com/en-us/aspnet/core/grpc/?view=aspnetcore-3.1