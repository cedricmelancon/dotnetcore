# Projet gRPC
Nous allons construire un Web API qui lance un appel gRPC qui lui revient.

1. Créer un projet de type Web API
2. Créer un API tel que défini dans la partie théorique (Greet).
3. Configurer le projet pour supporter gRPC.
4. Ajouter le service GreetService de type GreetBase
5. Créer un controller Greet qui envoie une requête gRPC et affiche la réponse.
6. À l'aide de Postman, valider que la requête est appelée (vous devriez obtenir le texte suivant dans la console)
```
Saying hello to <Your Name Here>
Hello <Your Name Here>
```