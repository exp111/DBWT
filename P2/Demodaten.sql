DELETE FROM Deklarationen;
DELETE FROM MahlzeitEnthältZutat;
DELETE FROM Zutaten;
DELETE FROM Fachbereiche;

REPLACE INTO `Deklarationen` (`Zeichen`, `Beschriftung`) VALUES
	('2', 'Konservierungsstoff'),
	('3', 'Antioxidationsmittel'),
	('4', 'Geschmacksverstärker'),
	('5', 'geschwefelt'),
	('6', 'geschwärzt'),
	('7', 'gewachst'),
	('8', 'Phosphat'),
	('9', 'Süßungsmittel'),
	('10', 'enthält eine Phenylalaninquelle'),
	('A', 'Gluten'),
	('A1', 'Weizen'),
	('A2', 'Roggen'),
	('A3', 'Gerste'),
	('A4', 'Hafer'),
	('A5', 'Dinkel'),
	('B', 'Sellerie'),
	('C', 'Krebstiere'),
	('D', 'Eier'),
	('E', 'Fische'),
	('F', 'Erdnüsse'),
	('G', 'Sojabohnen'),
	('H', 'Milch'),
	('I', 'Schalenfrüchte'),
	('I1', 'Mandeln'),
	('I2', 'Haselnüsse'),
	('I3', 'Walnüsse'),
	('I4', 'Kaschunüsse'),
	('I5', 'Pecannüsse'),
	('I6', 'Paranüsse'),
	('I7', 'Pistazien'),
	('I8', 'Macadamianüsse'),
	('J', 'Senf'),
	('K', 'Sesamsamen'),
	('L', 'Schwefeldioxid oder Sulfite'),
	('M', 'Lupinen'),
	('N', 'Weichtiere')
;

REPLACE INTO `Zutaten` (`ID`, `Name`, `Bio`, `Vegan`, `Vegetarisch`, `Glutenfrei`) VALUES
	(80, 'Aal', 0, 0, 0, 1),
	(81, 'Forelle', 0, 0, 0, 1),
	(82, 'Barsch', 0, 0, 0, 1),
	(83, 'Lachs', 0, 0, 0, 1),
	(84, 'Lachs', 1, 0, 0, 1),
	(85, 'Heilbutt', 0, 0, 0, 1),
	(86, 'Heilbutt', 1, 0, 0, 1),
	(100, 'Kurkumin', 1, 1, 1, 1),
	(101, 'Riboflavin', 0, 1, 1, 1),
	(123, 'Amaranth', 1, 1, 1, 1),
	(150, 'Zuckerkulör', 0, 1, 1, 1),
	(171, 'Titandioxid', 0, 1, 1, 1),
	(220, 'Schwefeldioxid', 0, 1, 1, 1),
	(270, 'Milchsäure', 0, 1, 1, 1),
	(322, 'Lecithin', 0, 1, 1, 1),
	(330, 'Zitronensäure', 1, 1, 1, 1),
	(999, 'Weizenmehl', 1, 1, 1, 0),
	(1000, 'Weizenmehl', 0, 1, 1, 0),
	(1001, 'Hanfmehl', 1, 1, 1, 1),
	(1010, 'Zucker', 0, 1, 1, 1),
	(1013, 'Traubenzucker', 0, 1, 1, 1),
	(1015, 'Branntweinessig', 0, 1, 1, 1),
	(2019, 'Karotten', 0, 1, 1, 1),
	(2020, 'Champignons', 0, 1, 1, 1),
	(2101, 'Schweinefleisch', 0, 0, 0, 1),
	(2102, 'Speck', 0, 0, 0, 1),
	(2103, 'Alginat', 0, 1, 1, 1),
	(2105, 'Paprika', 0, 1, 1, 1),
	(2107, 'Fenchel', 0, 1, 1, 1),
	(2108, 'Sellerie', 0, 1, 1, 1),
	(9020, 'Champignons', 1, 1, 1, 1),
	(9105, 'Paprika', 1, 1, 1, 1),
	(9107, 'Fenchel', 1, 1, 1, 1),
	(9110, 'Sojasprossen', 1, 1, 1, 1)
;


-- Im ER-Diagramm fehlt noch das Attribut Adresse, 
-- das Sie per ALTER TABLE einfach hinzufügen können
-- sobald Sie an den Punkt kommen ;)

REPLACE INTO `Fachbereiche` (`ID`, `Name`, `Website`, `Adresse`) VALUES
	(1, 'Architektur', 'https://www.fh-aachen.de/fachbereiche/architektur/', 'Bayernallee 9, 52066 Aachen'),
	(2, 'Bauingenieurwesen', 'https://www.fh-aachen.de/fachbereiche/bauingenieurwesen/', 'Bayernallee 9, 52066 Aachen'),
	(3, 'Chemie und Biotechnologie', 'https://www.fh-aachen.de/fachbereiche/chemieundbiotechnologie/', 'Heinrich-Mußmann-Straße 1, 52428 Jülich'),
	(4, 'Gestaltung', 'https://www.fh-aachen.de/fachbereiche/gestaltung/', 'Boxgraben 100, 52064 Aachen'),
	(5, 'Elektrotechnik und Informationstechnik', 'https://www.fh-aachen.de/fachbereiche/elektrotechnik-und-informationstechnik/', 'Eupener Straße 70, 52066 Aachen'),
	(6, 'Luft- und Raumfahrttechnik', 'https://www.fh-aachen.de/fachbereiche/luft-und-raumfahrttechnik/', 'Hohenstaufenallee 6, 52064 Aachen'),
	(7, 'Wirtschaftswissenschaften', 'https://www.fh-aachen.de/fachbereiche/wirtschaft/', 'Eupener Straße 70, 52066 Aachen'),
	(8, 'Maschinenbau und Mechatronik', 'https://www.fh-aachen.de/fachbereiche/maschinenbau-und-mechatronik/', 'Goethestraße 1, 52064 Aachen'),
	(9, 'Medizintechnik und Technomathematik', 'https://www.fh-aachen.de/fachbereiche/medizintechnik-und-technomathematik/', 'Heinrich-Mußmann-Straße 1, 52428 Jülich'),
	(10, 'Energietechnik', 'https://www.fh-aachen.de/fachbereiche/energietechnik/', 'Heinrich-Mußmann-Straße 1, 52428 Jülich')
;

