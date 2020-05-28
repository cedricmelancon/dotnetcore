# Projet jour 2
1.  Créer une application Web de type API App.
2.  Configurer le projet pour avoir les environnements suivants:
    1.  Production
    2.  Development
    3.  QA
3.  Ajoute un secret pour sauvegarder la connection string d'une base de données CosmosDB (SQL API).
4.  Créer un Model UserModel qui contient les champs suivants: FirstName, LastName, Address, PhoneNumber, Description.
5.  Créer un service UserService et ajoute-le au DI. Le service doit implémenter la fonction GetUsers qui retourne une liste de user sauvegardé dans une liste.
6.  Configurer le logging pour permettre par défaut le niveau Warning en production, Debug en développement et Information en QA.
7.  Créer un controller User et ajouter un appel HttpGet pour obtenir les users à partir du service UserService.
8.  Ajouter des logs des différents niveaux dans le service et le controller.
9.  Valider que l'application fonctionne correctement à l'aide de PostMan.