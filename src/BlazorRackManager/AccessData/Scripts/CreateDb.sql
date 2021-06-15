CREATE TABLE rack
(IdRack int NOT NULL AUTO_INCREMENT,
Gisement VARCHAR(5) NOT NULL,
PosRack VARCHAR(1) NOT NULL,
PRIMARY KEY(IdRack));

CREATE TABLE clients
(IdClient int NOT NULL AUTO_INCREMENT,
NomClient VARCHAR(50) NOT NULL,
PRIMARY KEY(IdClient));

CREATE TABLE mesure
(IdMesure int NOT NULL AUTO_INCREMENT,
Unite VARCHAR(10) NOT NULL,
PRIMARY KEY(IdMesure));

CREATE TABLE produit
(IdProduit VARCHAR(25) NOT NULL,
Nom VARCHAR(50) NOT NULL,
MesureId int NOT NULL,
ImgProduit LONGBLOB NULL,
FOREIGN KEY(MesureId) REFERENCES Mesure(IdMesure),
PRIMARY KEY(IdProduit));

CREATE TABLE suivicommande
(IdCommande int NOT NULL,
ClientId int NOT NULL,
DescriptionCmd VARCHAR(250), 
DateSortie DATETIME,
FOREIGN KEY(ClientId) REFERENCES Clients(IdClient),
PRIMARY KEY(IdCommande));

CREATE TABLE geocommande
(RackId int NOT NULL,
CommandeId int NOT NULL,
DateEntre datetime NOT NULL,
FOREIGN KEY(RackId) REFERENCES Rack(IdRack),
FOREIGN KEY(CommandeId) REFERENCES SuiviCommande(IdCommande),
PRIMARY KEY(RackId, CommandeId));

CREATE TABLE stock
(RackId int NOT NULL,
ProduitId VARCHAR(25) NOT NULL,
Quantite DOUBLE NOT NULL,
FOREIGN KEY(RackId) REFERENCES rack(IdRack),
FOREIGN KEY(ProduitId) REFERENCES produit(IdProduit),
PRIMARY KEY (RackId, ProduitId));

CREATE TABLE sortiestock
(ProduitId VARCHAR(25) NOT NULL,
Quantite DOUBLE NOT NULL,
DateSortie DATETIME NOT NULL,
FOREIGN KEY(ProduitId) REFERENCES produit(IdProduit),
PRIMARY KEY (ProduitId, DateSortie));