# Projet Kubernetes - Appeler un autre conteneur

## Télécharger l’exemple de code pour mywebapi
Pour des questions de temps, nous allons télécharger un exemple de code à partir d’un référentiel GitHub. Accédez à https://github.com/Azure/dev-spaces et sélectionnez Clone or Download (Cloner ou Télécharger) pour télécharger depuis le référentiel GitHub. Le code de cette section se trouve dans samples/dotnetcore/getting-started/mywebapi.

## Exécuter mywebapi
- Ouvrez le projet mywebapi dans une fenêtre distincte de Visual Studio.
- Sélectionnez Azure Dev Spaces dans la liste déroulante des paramètres de lancement comme vous l’avez déjà fait pour le projet webfrontend. Au lieu de créer un nouveau cluster ASK, sélectionnez celui que vous avez déjà créé. Comme avant, laissez le champ Space (Espace) sur default et cliquez sur OK. Dans la fenêtre Sortie, vous pouvez voir que Visual Studio commence à « préparer » ce nouveau service dans votre espace de développement afin d’accélérer les choses quand vous commencez à déboguer.
- Appuyez sur F5, et attendez la création et le déploiement du service. Lorsque la barre d’état de Visual Studio devient orange, le service est prêt.
- Notez l’URL du point de terminaison qui s’affiche dans le volet Azure Dev Spaces pour AKS de la fenêtre Sortie. Elle ressemble à ceci : http://localhost:<portnumber> Le conteneur a l’air de s’exécuter en local, mais en réalité, il s’exécute dans l’espace de développement dans Azure.
- Lorsque mywebapi est prêt, ouvrez votre navigateur à l’adresse localhost et ajoutez /api/values à l’URL pour appeler l’API GET par défaut pour le ValuesController.
- Si toutes les étapes se sont déroulées correctement, vous pouvez voir une réponse du service mywebapi qui ressemble à ceci.
  ```
  ["value1", "value2"]
  ```

## Effectuer une requête à partir de webfrontend sur mywebapi
Nous allons maintenant écrire du code dans webfrontend qui envoie une requête à mywebapi. Basculez vers la fenêtre Visual Studio qui contient le projet webfrontend. Dans le fichier HomeController.cs, remplacez le code de la méthode About par le code suivant :

```
public async Task<IActionResult> About()
{
   ViewData["Message"] = "Hello from webfrontend";

   using (var client = new System.Net.Http.HttpClient())
         {
             // Call *mywebapi*, and display its response in the page
             var request = new System.Net.Http.HttpRequestMessage();
             request.RequestUri = new Uri("http://mywebapi/api/values/1");
             if (this.Request.Headers.ContainsKey("azds-route-as"))
             {
                 // Propagate the dev space routing header
                 request.Headers.Add("azds-route-as", this.Request.Headers["azds-route-as"] as IEnumerable<string>);
             }
             var response = await client.SendAsync(request);
             ViewData["Message"] += " and " + await response.Content.ReadAsStringAsync();
         }

   return View();
}
```

L’exemple de code précédent transfère l’en-tête azds-route-as de la requête entrante à la requête sortante. Vous verrez plus tard comment cela permet d’augmenter la productivité dans les scénarios d’équipe.

## Déboguer dans plusieurs services
- À ce stade, mywebapi doit toujours être en cours d’exécution avec le débogueur joint. Si ce n’est pas le cas, appuyez sur F5 dans le projet mywebapi.
- Définissez un point d’arrêt dans la méthode Get(int id) du fichier Controllers/ValuesController.cs qui gère les demandes GET api/values/{id}.
- Dans le projet webfrontend où vous avez collé le code ci-dessus, définissez un point d’arrêt juste avant l’envoi d’une requête GET à mywebapi/api/values.
- Appuyez sur F5 dans le projet webfrontend. Visual Studio ouvre à nouveau un navigateur sur le port localhost approprié et l’application web s’affiche.
- Cliquez sur le lien « About » (À propos de) en haut de la page pour déclencher le point d’arrêt dans le projet webfrontend.
- Appuyez sur F10 pour continuer. Le point d’arrêt dans le projet mywebapi est maintenant déclenché.
- Appuyez sur F5 pour continuer et vous êtes redirigé dans le code du projet webfrontend.
- En appuyant encore une fois sur F5, vous terminez la requête et renvoyez une page dans le navigateur. Dans l’application web, la page À propos de affiche un message concaténé provenant des deux services : « Hello from webfrontend and Hello from mywebap ».