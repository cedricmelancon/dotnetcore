# Projet Kubernetes
- Assurez-vous d'avoir Azure CLI d'installé (https://docs.microsoft.com/fr-fr/cli/azure/install-azure-cli?view=azure-cli-latest)

# Créer un cluster Azure Kubernetes Service
Vous devez créer un cluster AKS dans une région prise en charge. Les commandes ci-dessous créent un groupe de ressources nommé MyResourceGroup et un cluster AKS nommé MyAKS.
Azure CLI

```
az group create --name MyResourceGroup --location eastus
az aks create -g MyResourceGroup -n MyAKS --location eastus --generate-ssh-keys
```

# Activer Azure Dev Spaces sur votre cluster AKS
Utilisez la commande use-dev-spaces pour activer Dev Spaces sur votre cluster AKS et suivez les invites. La commande ci-dessous active Dev Spaces sur le cluster MyAKS dans le groupe MyResourceGroup et crée un espace de développement par défaut.
 
```
az aks use-dev-spaces -g MyResourceGroup -n MyAKS
```

Résultat
```
'An Azure Dev Spaces Controller' will be created that targets resource 'MyAKS' in resource group 'MyResourceGroup'. Continue? (y/N): y

Creating and selecting Azure Dev Spaces Controller 'MyAKS' in resource group 'MyResourceGroup' that targets resource 'MyAKS' in resource group 'MyResourceGroup'...2m 24s

Select a dev space or Kubernetes namespace to use as a dev space.
 [1] default
Type a number or a new name: 1

Kubernetes namespace 'default' will be configured as a dev space. This will enable Azure Dev Spaces instrumentation for new workloads in the namespace. Continue? (Y/n): Y

Configuring and selecting dev space 'default'...3s

Managed Kubernetes cluster 'MyAKS' in resource group 'MyResourceGroup' is ready for development in dev space 'default'. Type `azds prep` to prepare a source directory for use with Azure Dev Spaces and `azds up` to run.
```

## Télécharger l’exemple de code pour mywebapi
Pour des questions de temps, nous allons télécharger un exemple de code à partir d’un référentiel GitHub. Accédez à https://github.com/Azure/dev-spaces et sélectionnez Clone or Download (Cloner ou Télécharger) pour télécharger depuis le référentiel GitHub. Le code de cette section se trouve dans samples/dotnetcore/getting-started/webfrontend.

# Connecter votre projet à votre espace de développement
Dans votre projet, sélectionnez Azure Dev Spaces dans la liste déroulante des paramètres de lancement, comme indiqué ci-dessous.

Dans la boîte de dialogue Azure Dev Spaces, sélectionnez votre abonnement et votre cluster Azure Kubernetes. Laissez Espace défini sur par défaut et cochez la case Accessible publiquement. Cliquez sur OK.

Ce processus déploie votre service sur l’espace de développement par défaut avec une URL accessible publiquement. Si vous choisissez un cluster qui n’a pas été configuré pour fonctionner avec Azure Dev Spaces, vous recevez un message vous demandant si vous souhaitez le configurer. Cliquez sur OK.

L’URL publique pour le service s’exécutant dans l’espace de développement par défaut est affichée dans la fenêtre Sortie :

```
Starting warmup for project 'webfrontend'.
Waiting for namespace to be provisioned.
Using dev space 'default' with target 'MyAKS'
...
Successfully built 1234567890ab
Successfully tagged webfrontend:devspaces-11122233344455566
Built container image in 39s
Waiting for container...
36s

Service 'webfrontend' port 'http' is available at `http://default.webfrontend.1234567890abcdef1234.eus.azds.io/`
Service 'webfrontend' port 80 (http) is available at http://localhost:62266
Completed warmup for project 'webfrontend' in 125 seconds.
```

Dans l’exemple ci-dessus, l’URL publique est http://default.webfrontend.1234567890abcdef1234.eus.azds.io/.
Sélectionnez Déboguer, puis Démarrer le débogage. Après quelques secondes, votre service démarre, et Visual Studio ouvre un navigateur avec l’URL publique du service. Si aucun navigateur ne s’ouvre automatiquement, accédez manuellement à l’URL publique de votre service à partir d’un navigateur, puis interagissez avec le service qui s’exécute dans votre espace de développement.
Ce processus peut avoir désactivé l’accès public à votre service. Pour activer l’accès public, vous pouvez mettre à jour la valeur entrée dans values.yaml.

# Mettre à jour le code
Si Visual Studio est toujours connecté à votre espace de développement, cliquez sur le bouton Arrêter. Changez le contenu de la méthode Index dans Controllers/HomeController.cs en :

```
public IActionResult Index()
{
    ViewData["Title"] = "My Home Page";
    return View();
}
```

Dans le fichier Index.cshtml, enlever le code suivant:

```
@{
    ViewData["Title"] = "Home Page";
}
```
Enregistrez les changements que vous avez apportés, sélectionnez Déboguer, puis Démarrer le débogage. Après quelques secondes, votre service démarre, et Visual Studio ouvre un navigateur avec l’URL publique du service. Si aucun navigateur ne s’ouvre automatiquement, accédez manuellement à l’URL publique de votre service à partir d’un navigateur. Votre mise-à-jour apparaît dans la barre de titre.

Au lieu de regénérer et de redéployer une nouvelle image conteneur chaque fois que des modifications de code sont effectuées, Azure Dev Spaces recompile le code de façon incrémentielle dans le conteneur existant afin d’accélérer la boucle de modification/débogage.