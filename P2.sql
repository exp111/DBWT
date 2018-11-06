USE dbwt;
-- RESET DB
#Zuerst Relationstabellen
DROP TABLE IF EXISTS MahlzeitHatBilder;
DROP TABLE IF EXISTS MahlzeitEnthältZutat;
DROP TABLE IF EXISTS MahlzeitBrauchtDeklaration;
DROP TABLE IF EXISTS Freundesliste;
DROP TABLE IF EXISTS BestellungenEnthältMahlzeiten;
DROP TABLE IF EXISTS FHAngehörigerGehörtZuFachbereich;

#Dann normale
DROP TABLE IF EXISTS Kommentare;

DROP TABLE IF EXISTS Mitarbeiter;
DROP TABLE IF EXISTS Studenten;
DROP TABLE IF EXISTS Gäste;
DROP TABLE IF EXISTS `FH Angehörige`;
DROP TABLE IF EXISTS Fachbereiche;

DROP TABLE IF EXISTS Preise;
DROP TABLE IF EXISTS Bestellungen;
DROP TABLE IF EXISTS Mahlzeiten;

DROP TABLE IF EXISTS Deklarationen;
DROP TABLE IF EXISTS Kategorien;
DROP TABLE IF EXISTS Bilder;
DROP TABLE IF EXISTS Zutaten;

DROP TABLE IF EXISTS Benutzer;

-- Create new tables
CREATE TABLE Benutzer (
  -- Columns
  Nummer INT NOT NULL AUTO_INCREMENT,
  `E-Mail` VARCHAR(255) NOT NULL, -- Soll unique sein
  Nutzername VARCHAR(255) NOT NULL, -- ebenso unique
  `Letzter Login` TIMESTAMP,
  Geburtsdatum DATE,
  Age INT AS (2018 - YEAR(Geburtsdatum)), #TODO
  Anlegedatum DATE NOT NULL,
  Aktiv BOOL NOT NULL, -- flag also bool
  -- Name
  Vorname VARCHAR(50) NOT NULL,
  Nachname VARCHAR(50) NOT NULL,
  -- Auth
  Salt CHAR(32) NOT NULL, -- immer 32 zeichen
  Hash CHAR(24) NOT NULL, -- immer 24 zeichen
  -- Misc
  PRIMARY KEY (Nummer),
  -- CONSTRAINT IstEin FOREIGN KEY (IsA) REFERENCES ,
  CONSTRAINT UniqueMail UNIQUE(`E-Mail`),
  CONSTRAINT UniqueUserName UNIQUE(Nutzername)
);

CREATE TABLE Gäste(
  Grund VARCHAR(255) NOT NULL,
  Ablaufdatum DATE NOT NULL,
  -- Relationen
  Nummer INT NOT NULL, -- [1,1]:[,1]
  -- Misc
  PRIMARY KEY (Nummer),
  CONSTRAINT GastIstEinBenutzer FOREIGN KEY (Nummer) REFERENCES Benutzer(Nummer) ON DELETE CASCADE
);

CREATE TABLE `FH Angehörige`(
  -- Relationen
  Nummer INT NOT NULL, -- [1,1]:[,1]
  -- Misc
  PRIMARY KEY (Nummer),
  CONSTRAINT FHIstEinBenutzer FOREIGN KEY (Nummer) REFERENCES Benutzer(Nummer) ON DELETE CASCADE
);

CREATE TABLE Fachbereiche(
  ID INT NOT NULL AUTO_INCREMENT,
  Name VARCHAR(100) NOT NULL,
  Website VARCHAR(255) NOT NULL,
  PRIMARY KEY (ID)
);

CREATE TABLE Mitarbeiter(
  Nummer INT NOT NULL,
  Telefon VARCHAR(15),
  Büro VARCHAR(10),
  -- Misc
  PRIMARY KEY (Nummer),
  FOREIGN KEY (Nummer) REFERENCES `FH Angehörige`(Nummer) ON DELETE CASCADE
);

CREATE TABLE Studenten(
  Nummer INT NOT NULL,
  Matrikelnummer INT NOT NULL,
  Studiengang ENUM('ET', 'INF', 'ISE', 'MCD', 'WI') NOT NULL,
  -- Misc
  PRIMARY KEY (Nummer),
  FOREIGN KEY (Nummer) REFERENCES `FH Angehörige`(Nummer) ON DELETE CASCADE,
  CONSTRAINT MatrikelUnique UNIQUE(Matrikelnummer),
  CONSTRAINT MatrikelAchtOderNeun CHECK(Matrikelnummer BETWEEN 10000000 AND 999999999)
);

CREATE TABLE Bestellungen(
  Nummer INT NOT NULL AUTO_INCREMENT,
  Bestellzeitpunkt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  Abholzeitpunkt TIMESTAMP CHECK (Abholzeitpunkt > Bestellzeitpunkt),
  Endpreis DOUBLE, # AS #TODO
  -- Relationen
  GetätigtVon INT NOT NULL, -- N:1
  -- Constraints
  PRIMARY KEY (Nummer),
  CONSTRAINT GetätigtVonBenutzer FOREIGN KEY (GetätigtVon) REFERENCES Benutzer(Nummer)
);

CREATE TABLE Bilder(
  ID INT NOT NULL AUTO_INCREMENT,
  AltText VARCHAR(255) NOT NULL,
  Title VARCHAR(50),
  Binärdaten BLOB NOT NULL,
  -- Misc
  PRIMARY KEY(ID)
);

CREATE TABLE Kategorien(
  ID INT NOT NULL AUTO_INCREMENT,
  Bezeichnung VARCHAR(50) NOT NULL,
  -- Relationen
  HatBilder INT, -- N:1
  Parent INT, #TODO: 1:N or N:1?
  -- Misc
  PRIMARY KEY (ID),
  FOREIGN KEY (Parent) REFERENCES Kategorien(ID),
  FOREIGN KEY (HatBilder) REFERENCES Bilder(ID)
);

CREATE TABLE Mahlzeiten(
  ID INT NOT NULL AUTO_INCREMENT,
  Beschreibung VARCHAR(255) NOT NULL,
  Vorrat INT NOT NULL DEFAULT 0,
  verfügbar BOOL AS (Vorrat > 0), -- flag
  -- Relationen
  inKategorie INT, -- N:1
  -- Misc
  PRIMARY KEY (ID),
  CONSTRAINT inKategorien FOREIGN KEY(inKategorie) REFERENCES Kategorien(ID)
);

CREATE TABLE Preise(
  ID INT NOT NULL,
  Jahr INT NOT NULL,
  Gastpreis DECIMAL(4,2) NOT NULL,
  `MAPreis` DECIMAL(4,2),
  Studentpreis DECIMAL(4,2) CHECK (`MAPreis` > Studentpreis),
  PRIMARY KEY (ID),
  FOREIGN KEY (ID) REFERENCES Mahlzeiten(ID) ON DELETE CASCADE
);

CREATE TABLE Zutaten(
  ID INT(5) NOT NULL,
  Name VARCHAR(50) NOT NULL,
  Bio BOOL NOT NULL,
  Vegetarisch BOOL NOT NULL,
  Vegan BOOL NOT NULL,
  Glutenfrei BOOL NOT NULL,
  PRIMARY KEY (ID)
);

CREATE TABLE Deklarationen(
  Zeichen VARCHAR(2) NOT NULL,
  Beschriftung VARCHAR(32) NOT NULL,
  PRIMARY KEY (Zeichen)
);

CREATE TABLE Kommentare(
  ID INT NOT NULL AUTO_INCREMENT,
  Bemerkung VARCHAR(255),
  Bewertung INT NOT NULL,
  -- Relationen
  zu INT NOT NULL, -- N:1
  Author INT NOT NULL,
  PRIMARY KEY (ID),
  CONSTRAINT GeschriebenVOn FOREIGN KEY (Author) REFERENCES Studenten(Nummer),
  CONSTRAINT KommentarZuMahlzeit FOREIGN KEY (zu) REFERENCES Mahlzeiten(ID)
);

-- Relationstabellen
CREATE TABLE Freundesliste(
  User INT NOT NULL,
  Freund INT NOT NULL,
  FOREIGN KEY (User) REFERENCES Benutzer(Nummer),
  FOREIGN KEY (Freund) REFERENCES Benutzer(Nummer)
);

CREATE TABLE BestellungenEnthältMahlzeiten(
  Anzahl INT NOT NULL,
  -- Relationen
  Bestellung INT NOT NULL,
  Mahlzeit INT NOT NULL,
  FOREIGN KEY (Bestellung) REFERENCES Bestellungen(Nummer),
  FOREIGN KEY (Mahlzeit) REFERENCES Mahlzeiten(ID)
);

CREATE TABLE MahlzeitHatBilder(
  Mahlzeit INT NOT NULL,
  Bild INT NOT NULL,
  FOREIGN KEY (Mahlzeit) REFERENCES Mahlzeiten(ID),
  FOREIGN KEY (Bild) REFERENCES Bilder(ID)
);

CREATE TABLE MahlzeitBrauchtDeklaration(
  Mahlzeit INT NOT NULL,
  Deklaration VARCHAR(32) NOT NULL,
  FOREIGN KEY (Mahlzeit) REFERENCES Mahlzeiten(ID),
  FOREIGN KEY (Deklaration) REFERENCES Deklarationen(Zeichen)
);

CREATE TABLE MahlzeitEnthältZutat(
  Mahlzeit INT NOT NULL,
  Zutat INT NOT NULL,
  FOREIGN KEY (Mahlzeit) REFERENCES Mahlzeiten(ID),
  FOREIGN KEY (Zutat) REFERENCES Zutaten(ID)
);

CREATE TABLE FHAngehörigerGehörtZuFachbereich(
  FHAngehöriger INT NOT NULL,
  Fachbereich INT NOT NULL,
  FOREIGN KEY (FHAngehöriger) REFERENCES `FH Angehörige`(Nummer),
  FOREIGN KEY (Fachbereich) REFERENCES Fachbereiche(ID)
);


-- Insert some Values
INSERT INTO Benutzer(`E-Mail`, Nutzername, Anlegedatum, Aktiv, Vorname, Nachname, Salt, Hash)
VALUES ("a@fh-aachen.de", "aaa", CURRENT_DATE, TRUE, "a", "a", 0, 0);
INSERT INTO Benutzer(`E-Mail`, Nutzername, Anlegedatum, Aktiv, Vorname, Nachname, Salt, Hash)
VALUES ("b@fh-aachen.de", "bab", CURRENT_DATE, FALSE, "b", "a", 1, 1);
INSERT INTO Benutzer(`E-Mail`, Nutzername, Anlegedatum, Aktiv, Vorname, Nachname, Salt, Hash)
VALUES ("c@fh-aachen.de", "ccc", CURRENT_DATE, FALSE, "c", "c", 2, 2);
INSERT INTO Benutzer(`E-Mail`, Nutzername, Anlegedatum, Aktiv, Vorname, Nachname, Salt, Hash)
VALUES ("d@fh-aachen.de", "ded", CURRENT_DATE, TRUE, "d", "e", 3, 3);

INSERT INTO `FH Angehörige`(Nummer) VALUES(1);
INSERT INTO `FH Angehörige`(Nummer) VALUES(2);
INSERT INTO `FH Angehörige`(Nummer) VALUES(4);

INSERT INTO Mitarbeiter(Nummer, Telefon) VALUES(1, "+49123456");
INSERT INTO Studenten(Nummer, Matrikelnummer, Studiengang) VALUES(2, 3144196, "INF");
INSERT INTO Studenten(Nummer, Matrikelnummer, Studiengang) VALUES(4, 123456789, "ET");

#DELETE FROM Benutzer WHERE Nummer = 2;
#DELETE FROM Studenten WHERE Studiengang = "ET";

SELECT * FROM Studenten WHERE Matrikelnummer BETWEEN 10000000 AND 999999999;