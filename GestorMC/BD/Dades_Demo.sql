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

-- Heroi (Jugable = TRUE)
INSERT INTO Personatge (id, atac, defensa, experiencia, velocitat, vida, nom, descripcio, imatge, icona, jugable) VALUES 
(1, 150, 100, 0, 110, 255, 'Jeffrey Epstein', 'Segueix viu en Palm Beach', 'epstein.png', 'epsteinIcon.png', TRUE),
(2, 200, 75, 0, 90, 240, 'Donald Trump', 'Demostra que tothom pot ser president', 'trump.png', 'trumpIcon.png', TRUE);

-- Enemics (Jugable = FALSE)
INSERT INTO Personatge (id, atac, defensa, experiencia, velocitat, vida, nom, descripcio, imatge, icona, jugable) VALUES 
(3, 45, 30, 20, 130, 50, 'Mercader Jueu', 'Extremadament ràpid amb els sheckels caiguts', 'jewBoss.png', 'jewIcon.PNG', FALSE),
(4, 180, 120, 150, 40, 250, 'Jueu Final Form', 'La victòria se li va prometre fa 6700 anys', 'megajewBoss.png', 'megajewIcon.PNG', FALSE);

-- =========================================================================
-- 3. HABILITATS (Màxim 4 per personatge - Trigger test)
-- =========================================================================


-- Accions per al Mercader Jueu (ID 3) i Jueu Final Form (ID 4)
INSERT INTO Accio (id, id_objectiu, nom, descripcio, icona) VALUES 
(60, 4, 'Plaga de Llagostes', 'Invoca un eixam que danya a tot l''equip enemic', 'llagostes.png'),
(61, 2, 'Clarí del Xofar', 'Un so sacre que pot atordir l''enemic', 'xofar.png'),
(62, 1, 'Pacte d''Abraham', 'Augmenta la defensa pròpia dràsticament', 'pacte.png'),
(63, 2, 'Interès Compost', 'Danya l''enemic i drena la seva velocitat', 'interes.png');

-- Assignació d'habilitats al Mercader Jueu (ID 3)
INSERT INTO Habilitat (id_accio, id_personatge) VALUES 
(61, 3), 
(63, 3);

-- Assignació d'habilitats al Jueu Final Form (ID 4)
INSERT INTO Habilitat (id_accio, id_personatge) VALUES 
(60, 4), 
(62, 4);


-- =========================================================================
-- 4. ÍTEMS I INVENTARI
-- =========================================================================


-- Crear el Vi de Quidush (Cura i bufa l'atac)
INSERT INTO Accio (id, id_objectiu, nom, descripcio, icona) VALUES 
(80, 1, 'Vi de Quidush', 'Vi sagrat que cura i dona força', 'vi.png');

INSERT INTO Item (id_accio) VALUES (80);


-- =========================================================================
-- 5. EFECTES (Lògica de Quantitat: Stat -> NULL | Estat -> NOT NULL)
-- =========================================================================


-- 60: Plaga de Llagostes (Dany de vida a tot l'equip enemic)
INSERT INTO Efecte (id, id_accio, nom, probabilitat, id_estadistica, id_estat, quantitat, duracio, esAfegir) VALUES 
(600, 60, 'Dany d''eixam', 100, 1, NULL, NULL, 0, FALSE);

-- 61: Clarí del Xofar (Aplica estat Atordit ID 3)
INSERT INTO Efecte (id, id_accio, nom, probabilitat, id_estadistica, id_estat, quantitat, duracio, esAfegir) VALUES 
(610, 61, 'Atordiment sonor', 60, NULL, 3, 1, 1, TRUE);

-- 62: Pacte d''Abraham (Puja Defensa ID 3)
INSERT INTO Efecte (id, id_accio, nom, probabilitat, id_estadistica, id_estat, quantitat, duracio, esAfegir) VALUES 
(620, 62, 'Protecció divina', 100, 3, NULL, 50, 3, TRUE);

-- 63: Interès Compost (Treu Vida + Baixa Velocitat ID 4)
INSERT INTO Efecte (id, id_accio, nom, probabilitat, id_estadistica, id_estat, quantitat, duracio, esAfegir) VALUES 
(630, 63, 'Cobrament', 100, 1, NULL, NULL, 0, FALSE),
(631, 63, 'Llast financer', 100, 4, NULL, 20, 2, FALSE);

-- Efectes del Vi: Cura Vida (NULL) i puja Atac (ID 2)
INSERT INTO Efecte (id, id_accio, nom, probabilitat, id_estadistica, id_estat, quantitat, duracio, esAfegir) VALUES 
(700, 80, 'Benedicció de salut', 100, 1, NULL, NULL, 0, TRUE),
(701, 80, 'Eufòria sagrada', 100, 2, NULL, 15, 2, TRUE);


-- =========================================================================
-- 6. NIVELLS I RECOMPENSES
-- =========================================================================

-- Nivell 1: Sinagoga (Ordre 1)
INSERT INTO Nivell (id, ordre, fons, id_enemic_1, id_enemic_2) VALUES 
(1, 1, 'level1.jpg', 3, 3);

-- Nivell 2: Infern Jueu (Ordre 2)
INSERT INTO Nivell (id, ordre, fons, id_enemic_1, id_enemic_2, id_enemic_3) VALUES 
(2, 2, 'level2.jpg', 3, 4, 3);

-- Recompenses: El vi es pot trobar a tots dos nivells
INSERT INTO Personatge_Item (id_personatge, id_item) VALUES 
(1, 80),
(2, 80);
