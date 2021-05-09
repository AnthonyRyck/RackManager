# RackManager
*(D�veloppement en cours)*

## A quoi elle sert ?
C'est une application en .Net Core Blazor, pour la gestion d'un entrepot. Elle permet de situer ou ce trouve chaque palette (*gisement*) par client et par commande.

Une image Docker est disponible sur [DockerHub](https://hub.docker.com/repository/docker/anthonyryck/rackmanager).  
Pour avoir la derni�re version pour tester, il faut prendre l'image avec le tag **ci**.

## Installation
Il faut cr�er un sch�ma sur MySQL avant d'ex�cuter l'application, ensuite c'est l'application qui va cr�er toutes les tables.

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

### Cr�er les racks  
Avant de stocker les palettes, il faut cr�er les racks, dans le menu Param�tres.

<img src="https://i.ibb.co/J7KQX8Y/01-Config-Racks.png">  

Et une image pour illustr� ce qu'est un gisement et une position.

<img src="https://i.ibb.co/dpNvXTZ/rack-Sample.jpg">

**BL** est une all�e dans le hangar. 


### Cr�er les clients.  
<img src="https://i.ibb.co/XF21kfD/02-Config-Client.png">


## Action sur l'entrepot/hangar
- possible de faire une entr�e d'une commande (1 � n palettes).  
<img src="https://i.ibb.co/cyxNWyM/03-Hangar-Entree.png">  
C'est au moment de l'entr� dans le rack qu'il faut saisir un num�ro de commande.
- faire une sortie de palette
- d�placer une palette vers un rack vide
- intervertir 2 palettes

## A venir
- un service d'API
- une application mobile