# Projet jour 3
À partir du projet du jour 2, exécutez les étapes suivantes:

1. Configurer Swagger pour le projet.
2. Configurer EntityFramework pour le projet et se connecter sur une base de données SQL provisionnée dans Azure. Le model est UserModel, mais cette fois-ci, au lieu d'avoir une seule adresse, nous allons mettre une liste d'adresse qui contient les champs suivants: ligne1, ligne2, city, province, country, zip code, description.
3. Modifier le UserService pour utiliser EntityFramework pour faire les requêtes sur la DB.
4. Ajouter les méthodes Add, Update et Delete au UserService
5. Ajouter les appels HttpPost, HttpPut et HttpDelete au controller
6. Valider à l'aide de Postman qu'il est possible de modifier la base de données avec les différents appels.

# Astuces
Si jamais vous avez l'erreur suivante lorsque vous ajouté votre migration EF

```
Add-Migration : The term 'Add-Migration' is not recognized as the name of a cmdlet, function, script file, or operable program. Check the spelling of the name, or if a path was included, verify that the path is correct and try again.
At line:1 char:1
+ Add-Migration InitialCreation
+ ~~~~~~~~~~~~~
    + CategoryInfo          : ObjectNotFound: (Add-Migration:String) [], CommandNotFoundException
    + FullyQualifiedErrorId : CommandNotFoundException
 
```

Exécutez la commande suivante:

```
Install-Package Microsoft.EntityFrameworkCore.Tools
```