DROP PROCEDURE IF EXISTS Nutzerrolle;
CREATE PROCEDURE Nutzerrolle(IN ID INT, OUT Role CHAR(25))
 BEGIN
   IF (SELECT COUNT(Nummer) FROM Studenten WHERE Nummer = ID) > 0 THEN
     SELECT 'Student' INTO Role;
   ELSEIF (SELECT COUNT(Nummer) FROM Mitarbeiter WHERE Nummer = ID) > 0 THEN
    SELECT 'Mitarbeiter' INTO Role;
   ELSEIF (SELECT COUNT(Nummer) FROM `fh angehörige` WHERE Nummer = ID) > 0 THEN
    SELECT 'FH Angehörige' INTO Role;
   ELSEIF (SELECT COUNT(Nummer) FROM Gäste WHERE Nummer = ID) > 0 THEN
    SELECT 'Gast' INTO Role;
   ELSEIF (SELECT COUNT(Nummer) FROM Benutzer WHERE Nummer = ID) > 0 THEN
    SELECT 'Benutzer' INTO Role;
   ELSE
     SELECT NULL INTO Role;
   END IF;
 END;

GRANT EXECUTE ON PROCEDURE dbwt.Nutzerrolle TO 'webapp'@'localhost';

DROP PROCEDURE IF EXISTS PreisFürNutzer;
CREATE PROCEDURE PreisFürNutzer(IN Nutzer INT, IN Mahlzeit INT)
  BEGIN
    CALL Nutzerrolle(Nutzer, @role);
    CASE @role
      WHEN "Mitarbeiter" THEN SELECT `MA-Preis` AS Preis FROM Preise WHERE ID = Mahlzeit;
      WHEN "FH Angehörige" THEN SELECT `MA-Preis` AS Preis FROM Preise WHERE ID = Mahlzeit;
      WHEN "Student" THEN SELECT Studentpreis AS Preis FROM Preise WHERE ID = Mahlzeit;
      ELSE SELECT Gastpreis AS Preis FROM Preise WHERE ID = Mahlzeit;
    END CASE;
  END;

GRANT EXECUTE ON PROCEDURE dbwt.PreisFürNutzer TO 'webapp'@'localhost';
CALL Nutzerrolle(2, @role);
SELECT @role;
CALL PreisFürNutzer(2,1);

GRANT INSERT(`E-Mail`, Nutzername, `Letzter Login`, Geburtsdatum, Anlegedatum, Aktiv, Vorname, Nachname, Salt, Hash) ON dbwt.Benutzer TO 'webapp'@'localhost'

SELECT       ID, Name, verfügbar, inKategorie, Vegetarisch, Vegan, `Alt-Text`, Titel, Binärdaten
FROM (SELECT Mahlzeiten.ID,
             Mahlzeiten.Name,
             Mahlzeiten.verfügbar,
             Mahlzeiten.inKategorie,
             (COUNT(mahlzeitenthältzutat.Zutat) = SUM(Zutaten.Vegetarisch)) AS Vegetarisch,
             (COUNT(mahlzeitenthältzutat.Zutat) = SUM(Zutaten.Vegan))       AS Vegan,
             Bilder.`Alt-Text`,
             Bilder.Titel,
             Bilder.Binärdaten
      FROM mahlzeiten
             LEFT JOIN MahlzeitEnthältZutat ON Mahlzeit = Mahlzeiten.ID
             LEFT JOIN Zutaten ON zutaten.ID = Zutat
             LEFT JOIN MahlzeitHatBilder ON mahlzeithatbilder.Mahlzeit = Mahlzeiten.ID
             LEFT JOIN Bilder ON mahlzeithatbilder.Bild = Bilder.ID
      GROUP BY Mahlzeiten.ID) AS Mahlzeiten;