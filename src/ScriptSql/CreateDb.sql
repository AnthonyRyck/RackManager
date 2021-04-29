CREATE TABLE Rack
(Gisement VARCHAR(5) NOT NULL,
Position VARCHAR(1) NOT NULL,
NomClient VARCHAR(50) NOT NULL,
Commande int NOT NULL,
DateEntre date NOT NULL,
PRIMARY KEY(Gisement, Position));