CREATE TABLE Rack
(IdRack int NOT NULL AUTO_INCREMENT,
Gisement VARCHAR(5) NOT NULL,
PosRack VARCHAR(1) NOT NULL,
PRIMARY KEY(IdRack));

CREATE TABLE Clients
(IdClient int NOT NULL AUTO_INCREMENT,
NomClient VARCHAR(50) NOT NULL,
PRIMARY KEY(IdClient));

CREATE TABLE SuiviCommande
(IdCommande int NOT NULL,
ClientId int NOT NULL,
DescriptionCmd VARCHAR(250), 
DateSortie DATETIME,
FOREIGN KEY(ClientId) REFERENCES Clients(IdClient),
PRIMARY KEY(IdCommande));

CREATE TABLE GeoCommande
(RackId int NOT NULL,
CommandeId int NOT NULL,
DateEntre datetime NOT NULL,
FOREIGN KEY(RackId) REFERENCES Rack(IdRack),
FOREIGN KEY(CommandeId) REFERENCES SuiviCommande(IdCommande),
PRIMARY KEY(RackId, CommandeId));