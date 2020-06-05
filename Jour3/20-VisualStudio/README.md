# Visual Studio
Visual Studio est l'outil principal d'un développeur .Net Core. Plusieurs outils sont disponibles pour accélérer le travail d'un développeur. Que ce soit par des touches de raccourcies, des extensions ou des plugins.

# Touches de raccourcies
## Commentaires
Une façon rapide de commenter un bloc de code est de le sélectionner et d'utiliser la combinaison CTRL-K, CTRL-C.

Pour décommenter ce même block, il faut utiliser la combinaison CTRL-K, CTRL-U

## Bookmarks
Les bookmarks sont utiles pour naviguer dans un fichier quand on travaille sur plusieurs fonctions. Pour mettre ou enlever un bookmark, il faut faire CTRL-K, CTRL-K. Pour naviguer d'un bookmark à l'autre, il faut utiliser CTRL-K, CTRL-N pour le suivant et CTRL-K, CTRL-P pour le précédent.

## Expand/Collapse d'un bloc de code
Comme vous le savez peut-être, il est possible de réduire certaines parties du code (à l'aide du + qui se trouve dans la marge gauche). Il est aussi possible de le faire par une combinaison de touches: CTRL-M, CTRL-M.

## Navigation
Il est possible de naviguer rapidement d'un fichier à l'autre sans avoir à utiliser la souris. Il suffit de faire CTRL-, et d'entrer soit le nom de la fonction ou du fichier que l'on veut naviguer vers.

## Reformater le fichier
Pour refaire l'indentation, au lieu de le faire manuellement, il suffit de faire les touches CTRL-K, CTRL-F.

## Camel Hump
Permet d'écrire un type en n'entrant que les premières lettres de chaque mot.

# Code Snippet
Il est possible de créer rapidement des portions de code en entrant un minimum de caractères.

- ctor: Création d'un constructeur
- prop: Création d'une propriété
- cw: Console.WriteLine
- equals: Génère le code pour un Equals
- try: génère le code pour un try/catch
- for/foreach/do/while: génère le code pour une boucle d'itération

# Plugin
Le plugin le plus populaire pour aider les développeurs est Resharper de la compagnie JetBrains. Il ajoute une panoblie d'outils pour accélérer le développement.
- Il permet d'analyser le code, détecte les optimisation et permet en une combinaison de touches (ALT-Enter) de les appliquer.
- Facilite le refactoring (Renommage de fichier, déplacement de fichier, etc.)

# Extensions
Plusieurs extensions sont disponibles sur le marketplace de Visual Studio (https://marketplace.visualstudio.com/)

Voici quelques extensions qui sont des incontournables:
- Add New File: Permet d'ajouter des fichiers/répertoires en faisant ALT-F2
- VSColorOutput: Met de la couleur dans la fenêtre d'output, ce qui rend plus facile de trouver les warning et les erreurs.
- Productivity Power Tools: Boîte à outils qui contient plusieurs fonctionnalités
    - AlignAssignment: CTRL-ALT-]
    - Double-click Maximize
    - Fix Mixed Tabs
    - Match Margin
    - Power Commands
- Spell Check: Valide l'ortographe dans votre code.