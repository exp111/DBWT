GRANT INSERT ON dbwt.Bestellungen TO 'webapp'@'localhost';
GRANT INSERT ON dbwt.BestellungenEnthältMahlzeiten TO 'webapp'@'localhost';

CREATE TRIGGER DecreaseVorratAfterBestellung
AFTER INSERT ON BestellungenEnthältMahlzeiten
FOR EACH ROW
UPDATE dbwt.mahlzeiten SET dbwt.mahlzeiten.Vorrat = dbwt.mahlzeiten.Vorrat - NEW.Anzahl WHERE dbwt.mahlzeiten.ID = NEW.Mahlzeit;