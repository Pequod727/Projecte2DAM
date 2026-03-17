-- =========================================================================
-- 1. POBLACIÓ DE CATÀLEGS (Els "Enums")
-- =========================================================================

INSERT INTO Objectiu (id, nom) VALUES 
(1, 'Tu'), 
(2, 'Enemic'), 
(3, 'Equip aliat'), 
(4, 'Equip enemic');

INSERT INTO Taula_estadistiques (id, nom, icona) VALUES 
(1, 'Vida', 'https://api.icon/hp.png'),
(2, 'Atac', 'https://api.icon/atk.png'),
(3, 'Defensa', 'https://api.icon/def.png'),
(4, 'Velocitat', 'https://api.icon/spd.png');

INSERT INTO Taula_estats (id, nom, descripcio, icona) VALUES 
(1, 'Enverinat', 'Dany per torn acumulat', 'https://api.icon/poison.png'),
(2, 'Cremat', 'Dany de foc i redueix atac', 'https://api.icon/fire.png'),
(3, 'Atordit', 'El personatge perd el torn', 'https://api.icon/stun.png');

-- =========================================================================
-- 2. PERSONATGES (Herois i Enemics)
-- =========================================================================

-- Heroi (Jugable = TRUE, icona = NULL per provar el fallback)
INSERT INTO Personatge (id, atac, defensa, experiencia, velocitat, vida, nom, descripcio, imatge, icona, jugable) VALUES 
(1, 150, 100, 0, 110, 255, 'Valeria la Valenta', 'Paladina del regne', 'valeria_full.png', NULL, TRUE);

-- Enemics (Jugable = FALSE)
INSERT INTO Personatge (id, atac, defensa, experiencia, velocitat, vida, nom, descripcio, imatge, icona, jugable) VALUES 
(2, 45, 30, 20, 130, 50, 'Gòblin Escolta', 'Ràpid i molest', 'goblin_sprite.png', 'goblin_ico.png', FALSE),
(3, 180, 120, 150, 40, 250, 'Troll de Pedra', 'Molt fort però lent', 'troll_sprite.png', 'troll_ico.png', FALSE);

-- =========================================================================
-- 3. HABILITATS (Màxim 4 per personatge - Trigger test)
-- =========================================================================

-- Habilitats de Valeria (ID 1)
INSERT INTO Accio (id, id_objectiu, nom, descripcio, icona) VALUES 
(10, 2, 'Cop Sagrat', 'Atac potent de llum', 'holy_hit.png'),
(11, 2, 'Estocada', 'Atac ràpid amb espasa', 'thrust.png'),
(12, 1, 'Pregària', 'Cura les ferides de l''usuari', 'heal.png'),
(13, 3, 'Escut Aura', 'Puja defensa a l''equip', 'aura.png');

INSERT INTO Habilitat (id_accio, id_personatge) VALUES 
(10, 1), (11, 1), (12, 1), (13, 1);

-- Nota: Si intentessis fer un INSERT d'una 5a habilitat per a l'ID 1, 
-- el trigger 'chk_maxim_habilitats' llançaria un error.

-- Habilitat del Gòblin (ID 2)
INSERT INTO Accio (id, id_objectiu, nom, descripcio, icona) VALUES 
(20, 2, 'Daga Enverinada', 'Punyalada amb verí', 'poison_dagger.png');
INSERT INTO Habilitat (id_accio, id_personatge) VALUES (20, 2);

-- =========================================================================
-- 4. EFECTES (Lògica de Quantitat: Stat -> NULL | Estat -> NOT NULL)
-- =========================================================================

-- Cop Sagrat: Treu Vida (id_stat=1, quantitat=NULL per petició)
INSERT INTO Efecte (id, id_accio, nom, probabilitat, id_estadistica, id_estat, quantitat, duracio, esAfegir) VALUES 
(100, 10, 'Dany Sagrat', 100, 1, NULL, NULL, 0, FALSE);

-- Daga Enverinada: Treu Vida + Aplica Estat Enverinat (id_estat=1, quantitat obligatòria)
INSERT INTO Efecte (id, id_accio, nom, probabilitat, id_estadistica, id_estat, quantitat, duracio, esAfegir) VALUES 
(200, 20, 'Tall físic', 100, 1, NULL, NULL, 0, FALSE),
(201, 20, 'Infecció', 75, NULL, 1, 15, 3, TRUE); -- Aplica estat 1 amb dany de 15

-- =========================================================================
-- 5. ÍTEMS I INVENTARI
-- =========================================================================

-- Crear un ítem (Poció)
INSERT INTO Accio (id, id_objectiu, nom, descripcio, icona) VALUES 
(50, 1, 'Poció X', 'Cura 100 HP', 'potion.png');
INSERT INTO Item (id_accio) VALUES (50);

-- Efecte de la poció: Surt una estadística (Vida), per tant quantitat és NULL
INSERT INTO Efecte (id, id_accio, nom, probabilitat, id_estadistica, id_estat, quantitat, duracio, esAfegir) VALUES 
(500, 50, 'Curació', 100, 1, NULL, NULL, 0, TRUE);

-- Valeria i el Gòblin tenen la mateixa poció (Relació M:N)
INSERT INTO Personatge_Item (id_personatge, id_item, quantitat_stock) VALUES 
(1, 50, 5),
(2, 50, 1);

-- =========================================================================
-- 6. NIVELLS I RECOMPENSES
-- =========================================================================

-- Nivell 1: Tutorial (Ordre 1)
INSERT INTO Nivell (id, ordre, fons, id_enemic_1, id_enemic_2) VALUES 
(1, 1, 'bosc_dia.jpg', 2, 2);

-- Nivell 2: La Cova (Ordre 2)
INSERT INTO Nivell (id, ordre, fons, id_enemic_1, id_enemic_2, id_enemic_3) VALUES 
(2, 2, 'cova_fosca.jpg', 2, 3, 2);

-- Recompenses: La Poció X es pot trobar a tots dos nivells
INSERT INTO Nivell_Item (id_nivell, id_item) VALUES 
(1, 50), 
(2, 50);