-- =========================================================================
-- 1. TAULES DE CATÀLEG
-- =========================================================================
CREATE TABLE Objectiu (
    id NUMERIC(1) PRIMARY KEY,
    nom VARCHAR(50) NOT NULL
);

CREATE TABLE Taula_estats (
    id NUMERIC(2) PRIMARY KEY,
    nom VARCHAR(50) NOT NULL,
    descripcio VARCHAR(255),
    icona VARCHAR(255) NOT NULL
);

CREATE TABLE Taula_estadistiques (
    id NUMERIC(2) PRIMARY KEY,
    nom VARCHAR(50) NOT NULL,
    icona VARCHAR(255) NOT NULL
);

-- =========================================================================
-- 2. ENTITAT PERSONATGE
-- =========================================================================
CREATE TABLE Personatge (
    id NUMERIC(3) PRIMARY KEY,
    atac NUMERIC(3) NOT NULL,
    defensa NUMERIC(3) NOT NULL,
    experiencia NUMERIC(3) NOT NULL,
    velocitat NUMERIC(3) NOT NULL,
    vida NUMERIC(3) NOT NULL,
    nom VARCHAR(100) NOT NULL,
    descripcio VARCHAR(255),
    imatge VARCHAR(255) NOT NULL,
    icona VARCHAR(255) DEFAULT NULL,
    jugable BOOLEAN NOT NULL,
    CONSTRAINT chk_stats_byte CHECK (
        atac BETWEEN 1 AND 255 AND 
        defensa BETWEEN 1 AND 255 AND 
        velocitat BETWEEN 1 AND 255 AND 
        vida BETWEEN 1 AND 255 AND 
        experiencia BETWEEN 0 AND 255
    )
);

-- =========================================================================
-- 3. ACCIONS, HABILITATS I ÍTEMS
-- =========================================================================

-- Cada acció té exactament 1 objectiu (Relació 1:N amb Objectiu)
CREATE TABLE Accio (
    id NUMERIC(3) PRIMARY KEY,
    id_objectiu NUMERIC(1) NOT NULL, -- Relació d'objectiu afegida
    nom VARCHAR(100) NOT NULL,
    descripcio VARCHAR(255),
    icona VARCHAR(255) NOT NULL,
    FOREIGN KEY (id_objectiu) REFERENCES Objectiu(id) ON UPDATE CASCADE
);

-- Cada habilitat és única d'un personatge
CREATE TABLE Habilitat (
    id_accio NUMERIC(3) PRIMARY KEY,
    id_personatge NUMERIC(3) NOT NULL,
    FOREIGN KEY (id_accio) REFERENCES Accio(id) ON DELETE CASCADE,
    FOREIGN KEY (id_personatge) REFERENCES Personatge(id) ON DELETE CASCADE
);

CREATE TABLE Item (
    id_accio NUMERIC(3) PRIMARY KEY,
    FOREIGN KEY (id_accio) REFERENCES Accio(id) ON DELETE CASCADE
);

-- Relació M:N entre personatges i ítems
CREATE TABLE Personatge_Item (
    id_personatge NUMERIC(3) NOT NULL,
    id_item NUMERIC(3) NOT NULL,
    quantitat_stock NUMERIC(2) DEFAULT 1,
    PRIMARY KEY (id_personatge, id_item),
    FOREIGN KEY (id_personatge) REFERENCES Personatge(id) ON DELETE CASCADE,
    FOREIGN KEY (id_item) REFERENCES Item(id_accio) ON DELETE CASCADE
);

-- =========================================================================
-- 4. SISTEMA D'EFECTES
-- =========================================================================
CREATE TABLE Efecte (
    id NUMERIC(3) PRIMARY KEY,
    id_accio NUMERIC(3) NOT NULL,
    nom VARCHAR(100) NOT NULL,
    probabilitat NUMERIC(3) DEFAULT 100,
    id_estat NUMERIC(2) DEFAULT NULL,
    id_estadistica NUMERIC(2) DEFAULT NULL,
    quantitat NUMERIC(3) DEFAULT NULL, 
    duracio NUMERIC(2) DEFAULT 0,
    esAfegir BOOLEAN NOT NULL,
    FOREIGN KEY (id_accio) REFERENCES Accio(id) ON DELETE CASCADE,
    FOREIGN KEY (id_estat) REFERENCES Taula_estats(id) ON DELETE SET NULL,
    FOREIGN KEY (id_estadistica) REFERENCES Taula_estadistiques(id) ON DELETE SET NULL,
    
    -- Lògica segons la teva petició:
    -- id_estadistica -> quantitat ha de ser NULL
    -- id_estat -> quantitat ha de ser NOT NULL
    CONSTRAINT chk_efecte_integritat CHECK (
        (id_estadistica IS NOT NULL AND id_estat IS NULL AND quantitat IS NULL) OR
        (id_estat IS NOT NULL AND id_estadistica IS NULL AND quantitat IS NOT NULL)
    )
);

-- =========================================================================
-- 5. NIVELLS (Món)
-- =========================================================================
CREATE TABLE Nivell (
    id NUMERIC(3) PRIMARY KEY,
    ordre NUMERIC(2) UNIQUE NOT NULL,
    fons VARCHAR(255),
    id_enemic_1 NUMERIC(3), 
    id_enemic_2 NUMERIC(3), 
    id_enemic_3 NUMERIC(3), 
    id_enemic_4 NUMERIC(3),
    CONSTRAINT chk_ordre_positiu CHECK (ordre > 0),
    FOREIGN KEY (id_enemic_1) REFERENCES Personatge(id) ON DELETE SET NULL,
    FOREIGN KEY (id_enemic_2) REFERENCES Personatge(id) ON DELETE SET NULL,
    FOREIGN KEY (id_enemic_3) REFERENCES Personatge(id) ON DELETE SET NULL,
    FOREIGN KEY (id_enemic_4) REFERENCES Personatge(id) ON DELETE SET NULL
);

CREATE TABLE Nivell_Item (
    id_nivell NUMERIC(3),
    id_item NUMERIC(3),
    PRIMARY KEY(id_nivell, id_item),
    FOREIGN KEY (id_nivell) REFERENCES Nivell(id) ON DELETE CASCADE,
    FOREIGN KEY (id_item) REFERENCES Item(id_accio) ON DELETE CASCADE
);

-- =========================================================================
-- 6. TRIGGERS
-- =========================================================================
DELIMITER $$

-- Màxim 4 habilitats per personatge
CREATE TRIGGER chk_maxim_habilitats BEFORE INSERT ON Habilitat
FOR EACH ROW 
BEGIN
    DECLARE total_hab INT;
    SELECT COUNT(*) INTO total_hab FROM Habilitat WHERE id_personatge = NEW.id_personatge;
    IF total_hab >= 4 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error: Aquest personatge ja té el màxim de 4 habilitats.';
    END IF;
END$$

-- Validació d'enemics en nivell (mínim 1)
CREATE TRIGGER chk_enemics_nivell_insert BEFORE INSERT ON Nivell
FOR EACH ROW 
BEGIN
    IF NEW.id_enemic_1 IS NULL AND NEW.id_enemic_2 IS NULL AND NEW.id_enemic_3 IS NULL AND NEW.id_enemic_4 IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error: El nivell ha de tenir almenys un enemic.';
    END IF;
END$$

-- Evitar deixar un nivell buit d'enemics
CREATE TRIGGER chk_enemics_nivell_update BEFORE UPDATE ON Nivell
FOR EACH ROW 
BEGIN
    IF NEW.id_enemic_1 IS NULL AND NEW.id_enemic_2 IS NULL AND NEW.id_enemic_3 IS NULL AND NEW.id_enemic_4 IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error: No pots deixar un nivell sense enemics.';
    END IF;
END$$

DELIMITER ;