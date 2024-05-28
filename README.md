# API de classement en tournoi

Cette API permet de gérer les classements et les résultats des tournois. Elle offre des fonctionnalités pour enregistrer les joueurs, gérer les scores et obtenir les classements d'un tournoi.

## Prérequis
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

## Installation

## 1. Cloner le dépôt

Clonez le dépôt sur votre machine locale :
```bash
git clone https://github.com/mcaravati/BetclicApi.git
cd BetclicApi
```

## 2. Configurer les variables d'environnement
Assurez-vous que la variable d'environnement `ASPNETCORE_ENVIRONMENT` est définie sur `Development` pour le développement local. Vous pouvez modifier ce paramètre dans `Properties/launchSettings.json`.

## 3. Restaurer les dépendances

Restaurez les dépendances du projet avec la commande suivante :
```bash
dotnet restore
```

## 4. Création de la base de données

Appliquez les migrations pour créer la base de données :
```bash
dotnet ef database update
```

## 5. Lancer l'application

Démarrez l'application avec la commande suivante :
```bash
dotnet run
```

## Mise en production

Avant de mettre l'application en production, veuillez effectuer les étapes suivantes :

1. **Passer à l'environnement de production :** Modifier `ASPNETCORE_ENVIRONMENT` à `Production` dans `Properties/launchSettings.json`.
2. **Configurer les hôtes autorisés :** Spécifier les hôtes autorisés à accéder à l'API dans `appsettings.json`.
3. **Configurer les certificats pour HTTPS**

## Pistes d'amélioration
- Ajouter un **filtre à injures** pour éviter que les utilisateurs en utilisent dans leurs noms d'utilisateurs.
- Ajouter un **système d'authentification** à l'API par token JWT / Bearer.
- **Changer le système de gestion de base de données** en fonction du nombre d'opérations effectuées à la minute. En effet, SQLite a été utilisé ici car le projet est de petite envergure et ne nécessite pas de temps de réaction rapide. Dans un cas d'utilisation réel, un autre système de gestion de base de données serait à considérer (ex: MySQL, PostgreSQL)