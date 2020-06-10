# Projet sur les Unit Tests

- Prendre le code se trouvant dans Code/UserApplication/UserApplication.
- Créer un projet de test unitaire pour tester ce projet (choisir le framework que vous voulez: MSTest, xUnit ou NUnit)
- Créer un test unitaire pour le UserService
  - Utiliser la librairie Moq pour remplacer certaines exécutions de code: https://github.com/Moq/moq4/wiki/Quickstart.
  - Faire un mock de IUnitOfWork pour retourner une implémentation de IRepository\<UserModel> pour abstraire l'accès à la base de données.
  - Tester au moins deux fonctions du service