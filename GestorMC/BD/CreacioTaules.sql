-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Temps de generació: 29-04-2026 a les 19:11:58
-- Versió del servidor: 10.4.28-MariaDB
-- Versió de PHP: 8.2.4

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de dades: `pollastre`
--

-- --------------------------------------------------------

--
-- Estructura de la taula `accio`
--

CREATE TABLE `accio` (
  `id` decimal(3,0) NOT NULL,
  `id_objectiu` decimal(1,0) NOT NULL,
  `nom` varchar(100) NOT NULL,
  `descripcio` varchar(255) DEFAULT NULL,
  `icona` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de la taula `efecte`
--

CREATE TABLE `efecte` (
  `id` decimal(3,0) NOT NULL,
  `id_accio` decimal(3,0) NOT NULL,
  `nom` varchar(100) NOT NULL,
  `probabilitat` decimal(3,0) DEFAULT 100,
  `id_estat` decimal(2,0) DEFAULT NULL,
  `id_estadistica` decimal(2,0) DEFAULT NULL,
  `quantitat` decimal(3,0) DEFAULT NULL,
  `duracio` decimal(2,0) DEFAULT 0,
  `esAfegir` tinyint(1) NOT NULL
) ;

-- --------------------------------------------------------

--
-- Estructura de la taula `habilitat`
--

CREATE TABLE `habilitat` (
  `id_accio` decimal(3,0) NOT NULL,
  `id_personatge` decimal(3,0) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Disparadors `habilitat`
--
DELIMITER $$
CREATE TRIGGER `chk_maxim_habilitats` BEFORE INSERT ON `habilitat` FOR EACH ROW BEGIN
    DECLARE total_hab INT;
    SELECT COUNT(*) INTO total_hab FROM Habilitat WHERE id_personatge = NEW.id_personatge;
    IF total_hab >= 4 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error: Aquest personatge ja té el màxim de 4 habilitats.';
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de la taula `item`
--

CREATE TABLE `item` (
  `id_accio` decimal(3,0) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de la taula `nivell`
--

CREATE TABLE `nivell` (
  `id` decimal(3,0) NOT NULL,
  `ordre` decimal(2,0) NOT NULL,
  `fons` varchar(255) DEFAULT NULL,
  `id_enemic_1` decimal(3,0) DEFAULT NULL,
  `id_enemic_2` decimal(3,0) DEFAULT NULL,
  `id_enemic_3` decimal(3,0) DEFAULT NULL,
  `id_enemic_4` decimal(3,0) DEFAULT NULL
) ;

--
-- Disparadors `nivell`
--
DELIMITER $$
CREATE TRIGGER `chk_enemics_nivell_insert` BEFORE INSERT ON `nivell` FOR EACH ROW BEGIN
    IF NEW.id_enemic_1 IS NULL AND NEW.id_enemic_2 IS NULL AND NEW.id_enemic_3 IS NULL AND NEW.id_enemic_4 IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error: El nivell ha de tenir almenys un enemic.';
    END IF;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `chk_enemics_nivell_update` BEFORE UPDATE ON `nivell` FOR EACH ROW BEGIN
    IF NEW.id_enemic_1 IS NULL AND NEW.id_enemic_2 IS NULL AND NEW.id_enemic_3 IS NULL AND NEW.id_enemic_4 IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error: No pots deixar un nivell sense enemics.';
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de la taula `nivell_item`
--

CREATE TABLE `nivell_item` (
  `id_nivell` decimal(3,0) NOT NULL,
  `id_item` decimal(3,0) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de la taula `objectiu`
--

CREATE TABLE `objectiu` (
  `id` decimal(1,0) NOT NULL,
  `nom` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de la taula `personatge`
--

CREATE TABLE `personatge` (
  `id` decimal(3,0) NOT NULL,
  `atac` decimal(3,0) NOT NULL,
  `defensa` decimal(3,0) NOT NULL,
  `experiencia` decimal(3,0) NOT NULL,
  `velocitat` decimal(3,0) NOT NULL,
  `vida` decimal(3,0) NOT NULL,
  `nom` varchar(100) NOT NULL,
  `descripcio` varchar(255) DEFAULT NULL,
  `imatge` varchar(255) NOT NULL,
  `icona` varchar(255) DEFAULT NULL,
  `jugable` tinyint(1) NOT NULL
) ;

-- --------------------------------------------------------

--
-- Estructura de la taula `personatge_item`
--

CREATE TABLE `personatge_item` (
  `id_personatge` decimal(3,0) NOT NULL,
  `id_item` decimal(3,0) NOT NULL,
  `quantitat_stock` decimal(2,0) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de la taula `taula_estadistiques`
--

CREATE TABLE `taula_estadistiques` (
  `id` decimal(2,0) NOT NULL,
  `nom` varchar(50) NOT NULL,
  `icona` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de la taula `taula_estats`
--

CREATE TABLE `taula_estats` (
  `id` decimal(2,0) NOT NULL,
  `nom` varchar(50) NOT NULL,
  `descripcio` varchar(255) DEFAULT NULL,
  `icona` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Índexs per a les taules bolcades
--

--
-- Índexs per a la taula `accio`
--
ALTER TABLE `accio`
  ADD PRIMARY KEY (`id`),
  ADD KEY `id_objectiu` (`id_objectiu`);

--
-- Índexs per a la taula `efecte`
--
ALTER TABLE `efecte`
  ADD PRIMARY KEY (`id`),
  ADD KEY `id_accio` (`id_accio`),
  ADD KEY `id_estat` (`id_estat`),
  ADD KEY `id_estadistica` (`id_estadistica`);

--
-- Índexs per a la taula `habilitat`
--
ALTER TABLE `habilitat`
  ADD PRIMARY KEY (`id_accio`),
  ADD KEY `id_personatge` (`id_personatge`);

--
-- Índexs per a la taula `item`
--
ALTER TABLE `item`
  ADD PRIMARY KEY (`id_accio`);

--
-- Índexs per a la taula `nivell`
--
ALTER TABLE `nivell`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `ordre` (`ordre`),
  ADD KEY `id_enemic_1` (`id_enemic_1`),
  ADD KEY `id_enemic_2` (`id_enemic_2`),
  ADD KEY `id_enemic_3` (`id_enemic_3`),
  ADD KEY `id_enemic_4` (`id_enemic_4`);

--
-- Índexs per a la taula `nivell_item`
--
ALTER TABLE `nivell_item`
  ADD PRIMARY KEY (`id_nivell`,`id_item`),
  ADD KEY `id_item` (`id_item`);

--
-- Índexs per a la taula `objectiu`
--
ALTER TABLE `objectiu`
  ADD PRIMARY KEY (`id`);

--
-- Índexs per a la taula `personatge`
--
ALTER TABLE `personatge`
  ADD PRIMARY KEY (`id`);

--
-- Índexs per a la taula `personatge_item`
--
ALTER TABLE `personatge_item`
  ADD PRIMARY KEY (`id_personatge`,`id_item`),
  ADD KEY `id_item` (`id_item`);

--
-- Índexs per a la taula `taula_estadistiques`
--
ALTER TABLE `taula_estadistiques`
  ADD PRIMARY KEY (`id`);

--
-- Índexs per a la taula `taula_estats`
--
ALTER TABLE `taula_estats`
  ADD PRIMARY KEY (`id`);

--
-- Restriccions per a les taules bolcades
--

--
-- Restriccions per a la taula `accio`
--
ALTER TABLE `accio`
  ADD CONSTRAINT `accio_ibfk_1` FOREIGN KEY (`id_objectiu`) REFERENCES `objectiu` (`id`) ON UPDATE CASCADE;

--
-- Restriccions per a la taula `efecte`
--
ALTER TABLE `efecte`
  ADD CONSTRAINT `efecte_ibfk_1` FOREIGN KEY (`id_accio`) REFERENCES `accio` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `efecte_ibfk_2` FOREIGN KEY (`id_estat`) REFERENCES `taula_estats` (`id`) ON DELETE SET NULL,
  ADD CONSTRAINT `efecte_ibfk_3` FOREIGN KEY (`id_estadistica`) REFERENCES `taula_estadistiques` (`id`) ON DELETE SET NULL;

--
-- Restriccions per a la taula `habilitat`
--
ALTER TABLE `habilitat`
  ADD CONSTRAINT `habilitat_ibfk_1` FOREIGN KEY (`id_accio`) REFERENCES `accio` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `habilitat_ibfk_2` FOREIGN KEY (`id_personatge`) REFERENCES `personatge` (`id`) ON DELETE CASCADE;

--
-- Restriccions per a la taula `item`
--
ALTER TABLE `item`
  ADD CONSTRAINT `item_ibfk_1` FOREIGN KEY (`id_accio`) REFERENCES `accio` (`id`) ON DELETE CASCADE;

--
-- Restriccions per a la taula `nivell`
--
ALTER TABLE `nivell`
  ADD CONSTRAINT `nivell_ibfk_1` FOREIGN KEY (`id_enemic_1`) REFERENCES `personatge` (`id`) ON DELETE SET NULL,
  ADD CONSTRAINT `nivell_ibfk_2` FOREIGN KEY (`id_enemic_2`) REFERENCES `personatge` (`id`) ON DELETE SET NULL,
  ADD CONSTRAINT `nivell_ibfk_3` FOREIGN KEY (`id_enemic_3`) REFERENCES `personatge` (`id`) ON DELETE SET NULL,
  ADD CONSTRAINT `nivell_ibfk_4` FOREIGN KEY (`id_enemic_4`) REFERENCES `personatge` (`id`) ON DELETE SET NULL;

--
-- Restriccions per a la taula `nivell_item`
--
ALTER TABLE `nivell_item`
  ADD CONSTRAINT `nivell_item_ibfk_1` FOREIGN KEY (`id_nivell`) REFERENCES `nivell` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `nivell_item_ibfk_2` FOREIGN KEY (`id_item`) REFERENCES `item` (`id_accio`) ON DELETE CASCADE;

--
-- Restriccions per a la taula `personatge_item`
--
ALTER TABLE `personatge_item`
  ADD CONSTRAINT `personatge_item_ibfk_1` FOREIGN KEY (`id_personatge`) REFERENCES `personatge` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `personatge_item_ibfk_2` FOREIGN KEY (`id_item`) REFERENCES `item` (`id_accio`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
