# RackManager

## A quoi elle sert ?
C'est une application en .Net Core Blazor, pour la gestion d'un entrepot. Elle permet de situer ou se trouve chaque palette (*gisement*) par client et par commande.

Une image Docker est disponible sur [DockerHub](https://hub.docker.com/repository/docker/anthonyryck/rackmanager).  
Pour avoir la dernière version pour tester, il faut prendre l'image avec le tag **ci**.

## Installation
Il faut créer un schéma sur MySQL avant d'exécuter l'application, ensuite c'est l'application qui va créer toutes les tables.

NOTE :  
Il faut que la base de donnée MySQL est la configuration :
```
[mysqld]
lower_case_table_names=1
```
Sinon aucune requête ne peut passer.

Commande docker run :
```
docker run -d -p 3030:80 \
-e LOGIN_DB=YourLoginDb \
-e PASSWORD_DB=yourPassWord \
-e DB_NAME=NameOfSchema \
-e DB_HOST=Ip_Or_UrlDatabase \
--name nameContainer anthonyryck/rackmanager:latest
```

Exemple de docker compose avec une utilisation avec [Traefik](https://doc.traefik.io/traefik/) *(pour les labels)*.
```yml
rackmng:
     image: anthonyryck/rackmanager:latest
     container_name: rackmanagertest
     hostname: rackmng
     expose :
       - 80
     environment:
       - LOGIN_DB=YourLoginDb
       - PASSWORD_DB=yourPassWord 
       - DB_NAME=NameOfSchema
       - DB_HOST=Ip_Or_UrlDatabase
     labels:
       - traefik.enable=true
       - traefik.http.routers.rac.rule=Host(`rackmanager.yourdomain.com`)
       - traefik.http.routers.rac.entrypoints=https
       - traefik.http.routers.rac.tls=true
       - traefik.http.routers.rac.tls.certresolver=letsencrypt
```

## Configuration

Il faut avoir un un rôle Admin ou Manage. Le rôle Admin permet de changer les rôles des autres utilisateurs.  
Compte par défaut Admin :  
login : root  
mot de passe : Azerty123!  

### Créer les racks  
Avant de stocker les palettes, il faut créer les racks, dans le menu Paramètres.

<img src="https://i.ibb.co/J7KQX8Y/01-Config-Racks.png">  

Et une image pour illustré ce qu'est un gisement et une position.

<img src="https://i.ibb.co/dpNvXTZ/rack-Sample.jpg">

**BL** est une allée dans le hangar. 


### Créer les clients.  
<img src="https://i.ibb.co/XF21kfD/02-Config-Client.png">


## Action sur l'entrepot/hangar
- possible de faire une entrée d'une commande (1 à n palettes).  
<img src="https://i.ibb.co/cyxNWyM/03-Hangar-Entree.png">  
C'est au moment de l'entrée dans le rack qu'il faut saisir un numéro de commande.  

- faire une sortie de palette
- déplacer une palette vers un rack vide
- intervertir 2 palettes

## A venir
- un service d'API
- une application mobile
