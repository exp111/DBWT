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

CREATE PROCEDURE NutzerrollePrint(IN ID INT)
  BEGIN
    CALL Nutzerrolle(ID, @a);
    SELECT @a AS Role;
  END;

GRANT EXECUTE ON PROCEDURE dbwt.Nutzerrolle TO 'webapp'@'localhost';
GRANT EXECUTE ON PROCEDURE dbwt.NutzerrollePrint TO 'webapp'@'localhost';

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
CALL NutzerrollePrint(2);
CALL PreisFürNutzer(2,1);