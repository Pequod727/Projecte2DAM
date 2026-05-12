-- =========================================================================
-- 1. TAULES DE CONFIGURACIÓ (SISTEMA) [IDs 1-99]
-- =========================================================================

INSERT INTO Objectiu (id, nom) VALUES 
(1, 'Tu'), 
(2, 'Enemic'), 
(3, 'Aliat'),
(4, 'Equip aliat'), 
(5, 'Equip enemic');

INSERT INTO Taula_estadistiques (id, nom, icona) VALUES 
(1, 'Vida', 'https://raw.githubusercontent.com/Pequod727/Projecte2DAM/refs/heads/master/ImagesMC/gameIcons/vida.png'),
(2, 'Atac', 'https://raw.githubusercontent.com/Pequod727/Projecte2DAM/refs/heads/master/ImagesMC/gameIcons/atac.png'),
(3, 'Defensa', 'https://raw.githubusercontent.com/Pequod727/Projecte2DAM/refs/heads/master/ImagesMC/gameIcons/def.png'),
(4, 'Velocitat', 'https://raw.githubusercontent.com/Pequod727/Projecte2DAM/refs/heads/master/ImagesMC/gameIcons/vel.png');

INSERT INTO Taula_estats (id, nom, descripcio, icona) VALUES 
(1, 'Enverinat', 'Dany per torn acumulat', 'https://api.icon/poison.png'),
(2, 'Cremat', 'Dany de foc i redueix atac', 'https://api.icon/fire.png'),
(3, 'Atordit', 'El personatge perd el torn', 'https://api.icon/stun.png');


-- =========================================================================
-- 2. PERSONATGES [IDs 100-199]
-- =========================================================================

-- Herois (Jugable = TRUE)
INSERT INTO Personatge (id, atac, defensa, experiencia, velocitat, vida, nom, descripcio, imatge, icona, jugable) VALUES 
(101, 150, 100, 0, 110, 255, 'Jeffrey Epstein', 'Segueix viu en Palm Beach', 
'https://raw.githubusercontent.com/Pequod727/Projecte2DAM/refs/heads/master/ImagesMC/sprites/epstein.png', 
'https://raw.githubusercontent.com/Pequod727/Projecte2DAM/refs/heads/master/ImagesMC/charIcons/epsteinIcon.png', TRUE),
(102, 200, 75, 0, 90, 240, 'Donald Trump', 'Demostra que tothom pot ser president', 
'https://raw.githubusercontent.com/Pequod727/Projecte2DAM/refs/heads/master/ImagesMC/sprites/trump.png', 
'https://raw.githubusercontent.com/Pequod727/Projecte2DAM/refs/heads/master/ImagesMC/charIcons/trumpIcon.png', TRUE);

-- Enemics (Jugable = FALSE)
INSERT INTO Personatge (id, atac, defensa, experiencia, velocitat, vida, nom, descripcio, imatge, icona, jugable) VALUES 
(103, 45, 30, 20, 130, 50, 'Mercader Jueu', 'Extremadament ràpid amb els sheckels caiguts', 
'https://raw.githubusercontent.com/Pequod727/Projecte2DAM/refs/heads/master/ImagesMC/sprites/jewBoss.png', 
'https://raw.githubusercontent.com/Pequod727/Projecte2DAM/refs/heads/master/ImagesMC/charIcons/jewIcon.PNG', FALSE),
(104, 180, 120, 150, 40, 250, 'Jueu Final Form', 'La victòria se li va prometre fa 6700 anys', 
'https://raw.githubusercontent.com/Pequod727/Projecte2DAM/refs/heads/master/ImagesMC/sprites/megajewBoss.png', 
'https://raw.githubusercontent.com/Pequod727/Projecte2DAM/refs/heads/master/ImagesMC/charIcons/megajewIcon.PNG', FALSE);


-- =========================================================================
-- 3. ACCIONS (HABILITATS I ÍTEMS) [IDs 200-299]
-- =========================================================================

-- Habilitats d'Epstein (Cobreix Enverinat i Atordit)
INSERT INTO Accio (id, id_objectiu, nom, descripcio, icona) VALUES 
(201, 2, 'Còctel a la Illa', 'Un combinat sospitós que enverina l''enemic', 'cocktail.png'),
(202, 2, 'Xantatge de Menors', 'Deixa l''enemic atordit per la pressió social', 'xantatge.png');

-- Habilitats de Trump (Cobreix Cremat i Atordit)
INSERT INTO Accio (id, id_objectiu, nom, descripcio, icona) VALUES 
(203, 2, 'Twitter Roast', 'Un tuit tan incendiari que crema l''enemic', 'twitter.png'),
(204, 2, 'Fake News', 'Confon l''enemic fins a atordir-lo', 'fakenews.png');

-- Habilitats Enemics
INSERT INTO Accio (id, id_objectiu, nom, descripcio, icona) VALUES 
(210, 5, 'Plaga de Llagostes', 'Invoca un eixam que danya a tot l''equip enemic', 'llagostes.png'),
(211, 2, 'Clarí del Xofar', 'Un so sacre que pot atordir l''enemic', 'xofar.png'),
(212, 1, 'Pacte d''Abraham', 'Augmenta la defensa pròpia dràsticament', 'pacte.png'),
(213, 2, 'Interès Compost', 'Danya l''enemic i drena la seva velocitat', 'interes.png');

-- Ítems
INSERT INTO Accio (id, id_objectiu, nom, descripcio, icona) VALUES 
(250, 3, 'Vi de Quidush', 'Vi sagrat que cura i dona força', 'vi.png');

INSERT INTO Item (id_accio) VALUES (250);


-- =========================================================================
-- 4. ASSIGNACIÓ D'HABILITATS (TAULA INTERMÈDIA)
-- =========================================================================

INSERT INTO Habilitat (id_accio, id_personatge) VALUES 
(201, 101), (202, 101), -- Epstein
(203, 102), (204, 102), -- Trump
(211, 103), (213, 103), -- Mercader
(210, 104), (212, 104); -- Final Form


-- =========================================================================
-- 5. EFECTES [IDs 300-399]
-- =========================================================================

-- Efectes Epstein
INSERT INTO Efecte (id, id_accio, nom, probabilitat, id_estadistica, id_estat, quantitat, duracio, esAfegir) VALUES 
(301, 201, 'Venè d''illa', 100, NULL, 1, 1, 3, TRUE),
(302, 202, 'Paràlisi social', 80, NULL, 3, 1, 1, TRUE);

-- Efectes Trump
INSERT INTO Efecte (id, id_accio, nom, probabilitat, id_estadistica, id_estat, quantitat, duracio, esAfegir) VALUES 
(303, 203, 'Burn verbal', 100, NULL, 2, 1, 2, TRUE),
(304, 204, 'Confusió mediàtica', 70, NULL, 3, 1, 1, TRUE);

-- Efectes Enemics
INSERT INTO Efecte (id, id_accio, nom, probabilitat, id_estadistica, id_estat, quantitat, duracio, esAfegir) VALUES 
(310, 210, 'Dany d''eixam', 100, 1, NULL, 40, 0, FALSE),
(311, 211, 'Atordiment sonor', 60, NULL, 3, 1, 1, TRUE),
(312, 212, 'Protecció divina', 100, 3, NULL, 50, 3, TRUE),
(313, 213, 'Cobrament', 100, 1, NULL, 30, 0, FALSE),
(314, 213, 'Llast financer', 100, 4, NULL, 20, 2, FALSE);

-- Efectes Ítems
INSERT INTO Efecte (id, id_accio, nom, probabilitat, id_estadistica, id_estat, quantitat, duracio, esAfegir) VALUES 
(350, 250, 'Benedicció de salut', 100, 1, NULL, 50, 0, TRUE),
(351, 250, 'Eufòria sagrada', 100, 2, NULL, 15, 2, TRUE);


-- =========================================================================
-- 6. NIVELLS I INVENTARI [IDs 400-499]
-- =========================================================================

-- Nivells
INSERT INTO Nivell (id, ordre, fons, id_enemic_1, id_enemic_2) VALUES 
(401, 1, 'level1.jpg', 103, 103);

INSERT INTO Nivell (id, ordre, fons, id_enemic_1, id_enemic_2, id_enemic_3) VALUES 
(402, 2, 'level2.jpg', 103, 104, 103);

-- Inventari (Jeffrey i Donald comencen amb Vi)
INSERT INTO Personatge_Item (id_personatge, id_item, quantitat_stock) VALUES 
(101, 250, 2),
(102, 250, 2);
