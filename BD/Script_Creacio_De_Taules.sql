-- =========================================================================
-- 1. TAULES DE CATÀLEG
-- =========================================================================
CREATE TABLE IF NOT EXISTS Objectiu (
    id NUMERIC(1) NOT NULL UNIQUE,
    nom VARCHAR(50) NOT NULL COMMENT 'aliat, yo, enemic, aliats, enemics',
    PRIMARY KEY(id)
);

CREATE TABLE IF NOT EXISTS Taula_estats (
    id NUMERIC(2) NOT NULL UNIQUE,
    nom VARCHAR(50) NOT NULL,
    descripcio VARCHAR(255),
    imatge VARCHAR(255) NOT NULL,
    PRIMARY KEY(id)
);

-- =========================================================================
-- 2. TAULES PRINCIPALS (Personatge i les seves Accions)
-- =========================================================================
CREATE TABLE IF NOT EXISTS Personatge (
    id NUMERIC(3) NOT NULL UNIQUE,
    atac NUMERIC(3) NOT NULL,
    defensa NUMERIC(3) NOT NULL,
    experiencia NUMERIC(3) NOT NULL,
    velocitat NUMERIC(3) NOT NULL,
    vida NUMERIC(3) NOT NULL,
    nom VARCHAR(100) NOT NULL,
    descripcio VARCHAR(255),
    imatge BLOB NOT NULL,
    jugable BOOLEAN NOT NULL,
    PRIMARY KEY(id),
    CONSTRAINT chk_stats_positius CHECK (atac >= 0 AND defensa >= 0 AND vida >= 0 AND velocitat >= 0)
);

CREATE TABLE IF NOT EXISTS Accio (
    id NUMERIC(3) NOT NULL UNIQUE,
    id_personatge NUMERIC(3) NOT NULL,
    PRIMARY KEY(id),
    CONSTRAINT fk_accio_personatge FOREIGN KEY (id_personatge) 
        REFERENCES Personatge(id) ON UPDATE CASCADE ON DELETE CASCADE
);

-- =========================================================================
-- 3. ESPECIALITZACIONS D'ACCIÓ
-- =========================================================================
CREATE TABLE IF NOT EXISTS Habilitat (
    id_accio NUMERIC(3) NOT NULL UNIQUE,
    PRIMARY KEY(id_accio),
    CONSTRAINT fk_habilitat_accio FOREIGN KEY (id_accio) 
        REFERENCES Accio(id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Item (
    id_accio NUMERIC(3) NOT NULL UNIQUE,
    PRIMARY KEY(id_accio),
    CONSTRAINT fk_item_accio FOREIGN KEY (id_accio) 
        REFERENCES Accio(id) ON UPDATE CASCADE ON DELETE CASCADE
);

-- =========================================================================
-- 4. TAULA EFECTE (Amb múltiples efectes per acció)
-- =========================================================================
CREATE TABLE IF NOT EXISTS Efecte (
    id NUMERIC(3) NOT NULL UNIQUE,
    nom VARCHAR(100) NOT NULL,
    descripcio VARCHAR(255),
    id_accio NUMERIC(3) NOT NULL,
    id_objectiu NUMERIC(1) NOT NULL, -- Corregit l'error ortogràfic "id_objeciu" del teu dibuix
    probabilitat NUMERIC(3) DEFAULT 100,
    PRIMARY KEY(id),
    CONSTRAINT chk_probabilitat CHECK (probabilitat BETWEEN 0 AND 100),
    CONSTRAINT fk_efecte_accio FOREIGN KEY (id_accio) 
        REFERENCES Accio(id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT fk_efecte_objectiu FOREIGN KEY (id_objectiu) 
        REFERENCES Objectiu(id) ON UPDATE CASCADE ON DELETE RESTRICT
);

-- =========================================================================
-- 5. ESPECIALITZACIONS D'EFECTE
-- =========================================================================
CREATE TABLE IF NOT EXISTS Canvi_estat (
    id_efecte NUMERIC(3) NOT NULL UNIQUE,
    id_estat NUMERIC(2) NOT NULL,
    PRIMARY KEY(id_efecte),
    CONSTRAINT fk_canviestat_efecte FOREIGN KEY (id_efecte) 
        REFERENCES Efecte(id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT fk_canviestat_taulaestats FOREIGN KEY (id_estat) 
        REFERENCES Taula_estats(id) ON UPDATE CASCADE ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS Modificador (
    id_efecte NUMERIC(3) NOT NULL UNIQUE,
    duracio_torns NUMERIC(1) DEFAULT 0 COMMENT 'si es zero es instantani',
    estadistica VARCHAR(20) NOT NULL COMMENT 'nom exacte de la estadistica',
    quantitat NUMERIC(3) NOT NULL,
    PRIMARY KEY(id_efecte),
    CONSTRAINT chk_duracio CHECK (duracio_torns >= 0),
    CONSTRAINT fk_modificador_efecte FOREIGN KEY (id_efecte) 
        REFERENCES Efecte(id) ON UPDATE CASCADE ON DELETE CASCADE
);

-- =========================================================================
-- 6. EL TRIGGER (Escut contra tenir més de 4 accions per personatge)
-- =========================================================================
DELIMITER //

CREATE TRIGGER chk_maxim_accions_personatge
BEFORE INSERT ON Accio
FOR EACH ROW
BEGIN
    DECLARE total_accions INT;
    
    -- Compta quantes accions té el personatge
    SELECT COUNT(*) INTO total_accions 
    FROM Accio 
    WHERE id_personatge = NEW.id_personatge;
    
    -- Si ja en té 4, bloquegem la inserció
    IF total_accions >= 4 THEN
        SIGNAL SQLSTATE '45000' 
        SET MESSAGE_TEXT = 'Error: Aquest personatge ja té el màxim de 4 accions permeses.';
    END IF;
END;
//

DELIMITER ;

