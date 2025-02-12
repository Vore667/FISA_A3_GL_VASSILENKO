# EasySave

## Description
EasySave est un logiciel de sauvegarde développé en C# avec .NET 8.0. Il permet d'effectuer des sauvegardes sécurisées et automatisées des fichiers des utilisateurs. Ce projet s'intègre dans la suite logicielle ProSoft et respecte la politique tarifaire de l'éditeur.

## Fonctionnalités principales
- Sauvegarde complète et incrémentale
- Interface utilisateur soignée et intuitive (Livrable 2)
- Configuration personnalisable
- Journalisation des opérations
- Compatibilité avec Windows

## Technologies utilisées
- **Langage** : C#
- **Framework** : .NET 8.0
- **Environnement de développement** : Visual Studio 2022+
- **Gestion de versions** : GitHub
- **Modélisation UML** : ArgoUML

## Installation
1. Cloner le dépôt GitHub :
   ```bash
   git clone https://github.com/votre-repo/EasySave.git
   ```
2. Ouvrir le projet avec Visual Studio 2022
4. Compiler et exécuter le projet

## Documentation
### Diagramme de cas d'utilisation
Le diagramme de cas d'utilisation représente l’ensemble des interactions possibles entre les utilisateurs et l’application. Il permet de visualiser les différentes fonctionnalités offertes par le système et d’identifier les rôles des différents acteurs.
Les fonctionnalités sont les suivantes : 
- Créer une sauvegarde
- Exécuter une sauvegarde
- Supprimer une sauvegarde
- Changer la langue
- Gérer les logs

### Diagramme d'activité
Le diagramme d'activité permet de représenter le déclenchement des évènements en fonction de l'état du système. Il sert également à modéliser des processus pouvant s'exécuter en parallèle.
L'utilisateu à trois possibilités:
- Lister les tâches de sauvegarde
- Créer une tâche de sauvegarde
- Supprimer une tâche de sauvegarde
### Diagramme de classes
Le diagramme de classes définit l’architecture du projet en décrivant les classes et leurs relations. Il assure une séparation des responsabilités claire et facilite la maintenance du code.

### Diagramme de séquence
Le diagramme de séquence permet de visualiser le déroulé des échanges entre les composants du système. Il met en évidence le flux des appels de méthodes et la communication entre les objets dans un ordre chronologique précis.
Les différents scénarios sont :
- Création d'une sauvegarde
- Exécution d'une sauvegarde
- Suppression d'une sauvegarde
- Gestion des logs
