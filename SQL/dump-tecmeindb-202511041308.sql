-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)
--
-- Host: localhost    Database: tecmeindb
-- ------------------------------------------------------
-- Server version	8.0.37

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `impuesto`
--

DROP TABLE IF EXISTS `impuesto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `impuesto` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Codigo` varchar(10) NOT NULL,
  `Descripcion` varchar(100) NOT NULL,
  `Porcentaje` decimal(5,2) DEFAULT NULL,
  `ValorFijo` decimal(18,2) DEFAULT NULL,
  `CodigoSri` varchar(5) DEFAULT NULL,
  `SecTipoImpuesto` int NOT NULL,
  `Vigente` tinyint(1) NOT NULL DEFAULT '1',
  `FechaCreacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `FechaModificacion` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Codigo` (`Codigo`),
  KEY `FK_Impuesto_TipoImpuesto` (`SecTipoImpuesto`),
  CONSTRAINT `FK_Impuesto_TipoImpuesto` FOREIGN KEY (`SecTipoImpuesto`) REFERENCES `tipoimpuesto` (`Secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `impuesto`
--

LOCK TABLES `impuesto` WRITE;
/*!40000 ALTER TABLE `impuesto` DISABLE KEYS */;
INSERT INTO `impuesto` VALUES (2,'001','IVA del 15%',15.00,0.00,'IVA15',1,1,'2025-08-15 12:51:47','2025-08-15 12:54:26'),(3,'002','IVA 0%',0.00,0.00,'IVA0',1,1,'2025-08-16 14:22:08','2025-10-03 09:06:54'),(5,'003','Impuesto BOD',10.00,0.00,'BOD',2,1,'2025-08-17 20:14:59',NULL);
/*!40000 ALTER TABLE `impuesto` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `canton`
--

DROP TABLE IF EXISTS `canton`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `canton` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `secProvincia` int NOT NULL,
  `codigo` varchar(5) DEFAULT NULL,
  `nombre` varchar(100) DEFAULT NULL,
  `estaActivo` smallint DEFAULT NULL,
  PRIMARY KEY (`secuencial`),
  KEY `Fk_Provincia_Canton` (`secProvincia`),
  CONSTRAINT `Fk_Provincia_Canton` FOREIGN KEY (`secProvincia`) REFERENCES `provincia` (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=223 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `canton`
--

LOCK TABLES `canton` WRITE;
/*!40000 ALTER TABLE `canton` DISABLE KEYS */;
INSERT INTO `canton` VALUES (1,1,'01','CUENCA',1),(2,1,'02','GIRON',1),(3,1,'03','GUALACEO',1),(4,1,'04','NABON',1),(5,1,'05','PAUTE',1),(6,1,'06','PUCARA',1),(7,1,'07','SAN FERNANDO',1),(8,1,'08','SANTA ISABEL',1),(9,1,'09','SIGSIG',1),(10,1,'10','ONIA',1),(11,1,'11','CHORDELEG',1),(12,1,'12','EL PAN',1),(13,1,'13','SEVILLA DE ORO',1),(14,1,'14','GUACHAPALA',1),(15,1,'15','CAMILO PONCE ENRIQUEZ',1),(16,2,'01','GUARANDA',1),(17,2,'02','CHILLANES',1),(18,2,'03','CHIMBO',1),(19,2,'04','ECHEANDIA',1),(20,2,'05','SAN MIGUEL',1),(21,2,'06','CALUMA',1),(22,2,'07','LAS NAVES',1),(23,3,'01','AZOGUES',1),(24,3,'02','BIBLIAN',1),(25,3,'03','CANIAR',1),(26,3,'04','LA TRONCAL',1),(27,3,'05','EL TAMBO',1),(28,3,'06','DELEG',1),(29,3,'07','SUSCAL',1),(30,4,'01','TULCAN',1),(31,4,'02','BOLIVAR',1),(32,4,'03','ESPEJO',1),(33,4,'04','MIRA',1),(34,4,'05','MONTUFAR',1),(35,4,'06','SAN PEDRO DE HUACA',1),(36,5,'01','LATACUNGA',1),(37,5,'02','LA MANA',1),(38,5,'03','PANGUA',1),(39,5,'04','PUJILI',1),(40,5,'05','SALCEDO',1),(41,5,'06','SAQUISILI',1),(42,5,'07','SIGCHOS',1),(43,6,'01','RIOBAMBA',1),(44,6,'02','ALAUSI',1),(45,6,'03','COLTA',1),(46,6,'04','CHAMBO',1),(47,6,'05','CHUNCHI',1),(48,6,'06','GUAMOTE',1),(49,6,'07','GUANO',1),(50,6,'08','PALLATANGA',1),(51,6,'09','PENIPE',1),(52,6,'10','CUMANDA',1),(53,7,'01','MACHALA',1),(54,7,'02','ARENILLAS',1),(55,7,'03','ATAHUALPA',1),(56,7,'04','BALSAS',1),(57,7,'05','CHILLA',1),(58,7,'06','EL GUABO',1),(59,7,'07','HUAQUILLAS',1),(60,7,'08','MARCABELI',1),(61,7,'09','PASAJE',1),(62,7,'10','PINIAS',1),(63,7,'11','PORTOVELO',1),(64,7,'12','SANTA ROSA',1),(65,7,'13','ZARUMA',1),(66,7,'14','LAS LAJAS',1),(67,8,'01','ESMERALDAS',1),(68,8,'02','ELOY ALFARO',1),(69,8,'03','MUISNE',1),(70,8,'04','QUININDE',1),(71,8,'05','SAN LORENZO',1),(72,8,'06','ATACAMES',1),(73,8,'07','RIOVERDE',1),(74,9,'01','GUAYAQUIL',1),(75,9,'02','ALFREDO BAQUERIZO MORENO (JUJAN)',1),(76,9,'03','BALAO',1),(77,9,'04','BALZAR',1),(78,9,'05','COLIMES',1),(79,9,'06','DAULE',1),(80,9,'07','DURAN',1),(81,9,'08','EL EMPALME',1),(82,9,'09','EL TRIUNFO',1),(83,9,'10','MILAGRO',1),(84,9,'11','NARANJAL',1),(85,9,'12','NARANJITO',1),(86,9,'13','PALESTINA',1),(87,9,'14','PEDRO CARBO',1),(88,9,'16','SAMBORONDON',1),(89,9,'18','SANTA LUCIA',1),(90,9,'19','SALITRE',1),(91,9,'20','SAN JACINTO DE YAGUACHI',1),(92,9,'21','PLAYAS',1),(93,9,'22','SIMON BOLIVAR',1),(94,9,'23','CORONEL MARCELINO MARIDUENIA',1),(95,9,'24','LOMAS DE SARGENTILLO',1),(96,9,'25','NOBOL',1),(97,9,'27','GENERAL  ANTONIO ELIZALDE',1),(98,9,'28','ISIDRO AYORA',1),(99,10,'01','IBARRA',1),(100,10,'02','ANTONIO ANTE',1),(101,10,'03','COTACACHI',1),(102,10,'04','OTAVALO',1),(103,10,'05','PIMAMPIRO',1),(104,10,'06','SAN MIGUEL DE URCUQUI',1),(105,11,'01','LOJA',1),(106,11,'02','CALVAS',1),(107,11,'03','CATAMAYO',1),(108,11,'04','CELICA',1),(109,11,'05','CHAGUARPAMBA',1),(110,11,'06','ESPINDOLA',1),(111,11,'07','GONZANAMA',1),(112,11,'08','MACARA',1),(113,11,'09','PALTAS',1),(114,11,'10','PUYANGO',1),(115,11,'11','SARAGURO',1),(116,11,'12','SOZORANGA',1),(117,11,'13','ZAPOTILLO',1),(118,11,'14','PINDAL',1),(119,11,'15','QUILANGA',1),(120,11,'16','OLMEDO',1),(121,12,'01','BABAHOYO',1),(122,12,'02','BABA',1),(123,12,'03','MONTALVO',1),(124,12,'04','PUEBLOVIEJO',1),(125,12,'05','QUEVEDO',1),(126,12,'06','URDANETA',1),(127,12,'07','VENTANAS',1),(128,12,'08','VINCES',1),(129,12,'09','PALENQUE',1),(130,12,'10','BUENA FE',1),(131,12,'11','VALENCIA',1),(132,12,'12','MOCACHE',1),(133,12,'13','QUINSALOMA',1),(134,13,'01','PORTOVIEJO',1),(135,13,'02','BOLIVAR',1),(136,13,'03','CHONE',1),(137,13,'04','EL CARMEN',1),(138,13,'05','FLAVIO ALFARO',1),(139,13,'06','JIPIJAPA',1),(140,13,'07','JUNIN',1),(141,13,'08','MANTA',1),(142,13,'09','MONTECRISTI',1),(143,13,'10','PAJAN',1),(144,13,'11','PICHINCHA',1),(145,13,'12','ROCAFUERTE',1),(146,13,'13','SANTA ANA',1),(147,13,'14','SUCRE',1),(148,13,'15','TOSAGUA',1),(149,13,'16','24 DE MAYO',1),(150,13,'17','PEDERNALES',1),(151,13,'18','OLMEDO',1),(152,13,'19','PUERTO LOPEZ',1),(153,13,'20','JAMA',1),(154,13,'21','JARAMIJO',1),(155,13,'22','SAN VICENTE',1),(156,14,'01','MORONA',1),(157,14,'02','GUALAQUIZA',1),(158,14,'03','LIMON INDANZA',1),(159,14,'04','PALORA',1),(160,14,'05','SANTIAGO',1),(161,14,'06','SUCUA',1),(162,14,'07','HUAMBOYA',1),(163,14,'08','SAN JUAN BOSCO',1),(164,14,'09','TAISHA',1),(165,14,'10','LOGRONIO',1),(166,14,'11','PABLO SEXTO',1),(167,14,'12','TIWINTZA',1),(168,15,'01','TENA',1),(169,15,'03','ARCHIDONA',1),(170,15,'04','EL CHACO',1),(171,15,'07','QUIJOS',1),(172,15,'09','CARLOS JULIO AROSEMENA TOLA',1),(173,16,'01','PASTAZA',1),(174,16,'02','MERA',1),(175,16,'03','SANTA CLARA',1),(176,16,'04','ARAJUNO',1),(177,17,'01','DISTRITO METROPOLITANO DE QUITO',1),(178,17,'02','CAYAMBE',1),(179,17,'03','MEJIA',1),(180,17,'04','PEDRO MONCAYO',1),(181,17,'05','RUMINIAHUI',1),(182,17,'07','SAN MIGUEL DE LOS BANCOS',1),(183,17,'08','PEDRO VICENTE MALDONADO',1),(184,17,'09','PUERTO QUITO',1),(185,18,'01','AMBATO',1),(186,18,'02','BANIOS DE AGUA SANTA',1),(187,18,'03','CEVALLOS',1),(188,18,'04','MOCHA',1),(189,18,'05','PATATE',1),(190,18,'06','QUERO',1),(191,18,'07','SAN PEDRO DE PELILEO',1),(192,18,'08','SANTIAGO DE PILLARO',1),(193,18,'09','TISALEO',1),(194,19,'01','ZAMORA',1),(195,19,'02','CHINCHIPE',1),(196,19,'03','NANGARITZA',1),(197,19,'04','YACUAMBI',1),(198,19,'05','YANTZAZA',1),(199,19,'06','EL PANGUI',1),(200,19,'07','CENTINELA DEL CONDOR',1),(201,19,'08','PALANDA',1),(202,19,'09','PAQUISHA',1),(203,20,'01','SAN CRISTOBAL',1),(204,20,'02','ISABELA',1),(205,20,'03','SANTA CRUZ',1),(206,21,'01','LAGO AGRIO',1),(207,21,'02','GONZALO PIZARRO',1),(208,21,'03','PUTUMAYO',1),(209,21,'04','SHUSHUFINDI',1),(210,21,'05','SUCUMBIOS',1),(211,21,'06','CASCALES',1),(212,21,'07','CUYABENO',1),(213,22,'01','FRANCISCO DE ORELLANA',1),(214,22,'02','AGUARICO',1),(215,22,'03','LA JOYA DE LOS SACHAS',1),(216,22,'04','LORETO',1),(217,23,'01','SANTO DOMINGO',1),(218,23,'02','LA CONCORDIA',1),(219,24,'01','SANTA ELENA',1),(220,24,'02','LA LIBERTAD',1),(221,24,'03','SALINAS',1),(222,8,'08','LA CONCORDIA',1);
/*!40000 ALTER TABLE `canton` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `empresacorreo`
--

DROP TABLE IF EXISTS `empresacorreo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `empresacorreo` (
  `SecEmpresa` int NOT NULL,
  `email` varchar(150) DEFAULT NULL,
  `clave` varchar(255) DEFAULT NULL,
  `alias` varchar(50) DEFAULT NULL,
  `host` varchar(50) DEFAULT NULL,
  `puerto` int DEFAULT NULL,
  `estaActivo` smallint DEFAULT NULL,
  PRIMARY KEY (`SecEmpresa`),
  CONSTRAINT `Fk_Empresa_EmpCorreo` FOREIGN KEY (`SecEmpresa`) REFERENCES `empresa` (`secuencial`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `empresacorreo`
--

LOCK TABLES `empresacorreo` WRITE;
/*!40000 ALTER TABLE `empresacorreo` DISABLE KEYS */;
INSERT INTO `empresacorreo` VALUES (1,'tsi_notificacion@tecmein.com','CeronCadena1010@','Tecmein TSI','mail.tecmein.com',465,1);
/*!40000 ALTER TABLE `empresacorreo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `menu`
--

DROP TABLE IF EXISTS `menu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `menu` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(130) DEFAULT NULL,
  `secMenuPadre` int DEFAULT NULL,
  `icono` varchar(30) DEFAULT NULL,
  `controlador` varchar(130) DEFAULT NULL,
  `paginaAccion` varchar(130) DEFAULT NULL,
  `esActivo` smallint DEFAULT NULL,
  `fechaRegistro` datetime DEFAULT CURRENT_TIMESTAMP,
  `MostrarEnMenu` bit(1) NOT NULL DEFAULT b'0',
  `Orden` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`secuencial`),
  KEY `FK_Menu_Menu_idx` (`secMenuPadre`),
  CONSTRAINT `FK_Menu_Menu` FOREIGN KEY (`secMenuPadre`) REFERENCES `menu` (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=50 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `menu`
--

LOCK TABLES `menu` WRITE;
/*!40000 ALTER TABLE `menu` DISABLE KEYS */;
INSERT INTO `menu` VALUES (1,'DashBoard',NULL,'fas fa-fw fa-tachometer-alt','DashBoard','Index',1,NULL,_binary '',0),(2,'Administración',NULL,'fas fa-fw fa-cog',NULL,NULL,1,'2024-06-14 12:33:13',_binary '',0),(3,'Inventario',NULL,'fas fa-fw fa-clipboard-list',NULL,NULL,1,'2024-06-14 12:33:13',_binary '',0),(4,'Ventas',NULL,'fas fa-fw fa-tags',NULL,NULL,1,'2024-06-14 12:33:13',_binary '',0),(6,'Usuarios',2,NULL,'Usuario','Index',1,'2024-06-14 12:35:28',_binary '',0),(7,'Empresa',2,NULL,'Empresa','Index',1,'2024-06-14 12:35:28',_binary '',0),(10,'Visita',4,'','Visita','Index',1,'2024-06-26 20:05:10',_binary '',1),(12,'Constructora',2,NULL,'Constructora','Index',1,'2024-08-15 09:22:25',_binary '',0),(13,'Contactos',2,'','Contacto','Index',1,'2024-09-03 12:51:57',_binary '',0),(15,'Rol',17,'','Rol','Index',1,'2025-05-21 00:00:00',_binary '',0),(16,'Menu',17,'','Menu','Index',1,'2025-06-30 10:17:33',_binary '',0),(17,'Parámetros',NULL,'fas fa-cogs','','',1,'2025-06-30 11:06:17',_binary '',0),(18,'Rol Menu',17,'','RolMenu','Index',1,'2025-07-01 01:03:17',_binary '',0),(19,'Cotizaciones',4,'','Cotizacion','Index',1,'2025-08-07 12:50:44',_binary '',2),(20,'Impuestos',2,'','Impuesto','Index',1,'2025-08-12 10:49:24',_binary '',0),(21,'Tipo Impuesto',17,'','TipoImpuesto','Index',1,'2025-08-12 11:56:53',_binary '',0),(23,'Permisos',2,NULL,'Permiso','Index',1,'2025-08-24 10:15:03',_binary '',0),(24,'Pantillas Contratos',17,'','PlantillaPreContrato','Index',1,'2025-08-30 13:23:31',_binary '',0),(25,'Diccionario Parámetros',17,'','DiccionarioParametro','Index',1,'2025-09-03 10:44:42',_binary '',0),(27,'Tipo Documento',17,'','TipoDocumento','Index',1,'2025-09-04 10:01:27',_binary '',0),(29,'Poliza',2,'','PolizaGarantia','Index',1,'2025-09-17 12:56:44',_binary '',0),(31,'Numeros Cliente',2,'','FormatoNumeroCliente','Index',1,'2025-09-22 09:32:16',_binary '',0),(32,'Cliente',2,'fas fa-user','Cliente','Index',1,'2025-09-22 13:06:06',_binary '',0),(41,'Contrato',4,'fas fa-file','Contrato','Index',1,'2025-10-20 12:05:24',_binary '',4),(42,'Módulos sin Asignar',NULL,'fas fa-qrcode','','',1,'2025-10-23 12:17:00',_binary '',0),(43,'PreContrato',4,'fas fa-clipboard-list','PreContrato','Index',1,'2025-10-25 13:38:50',_binary '',3),(44,'Catalogo',3,'','Catalogo','Index',1,'2025-10-25 13:38:50',_binary '',0),(45,'Dashboard',42,'fa-puzzle-piece','Dashboard','Index',1,'2025-10-25 13:38:50',_binary '',0),(46,'FormaPago',2,NULL,'FormaPago','Index',1,'2025-10-25 13:38:50',_binary '',0),(47,'Plantilla',42,'fas fa-band-aid','Plantilla','Index',1,'2025-10-25 13:38:50',_binary '',0),(48,'PlantillaPreContratoParrafo',42,'fas fa-receipt','PlantillaPreContratoParrafo','Index',1,'2025-10-25 13:38:50',_binary '',0),(49,'Seguimiento',19,'','Seguimiento','Index',1,'2025-10-25 13:38:50',_binary '',0);
/*!40000 ALTER TABLE `menu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tipoimpuesto`
--

DROP TABLE IF EXISTS `tipoimpuesto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tipoimpuesto` (
  `Secuencial` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) NOT NULL,
  `EsIva` tinyint(1) NOT NULL DEFAULT '0',
  `EsImportacion` tinyint(1) NOT NULL DEFAULT '0',
  `EstaActivo` tinyint(1) NOT NULL DEFAULT '1',
  `FechaCreacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `FechaModificacion` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tipoimpuesto`
--

LOCK TABLES `tipoimpuesto` WRITE;
/*!40000 ALTER TABLE `tipoimpuesto` DISABLE KEYS */;
INSERT INTO `tipoimpuesto` VALUES (1,'IVA',1,0,1,'2025-08-12 12:00:37',NULL),(2,'BOD',0,1,1,'2025-08-17 20:12:59',NULL);
/*!40000 ALTER TABLE `tipoimpuesto` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rol`
--

DROP TABLE IF EXISTS `rol`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rol` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(150) DEFAULT NULL,
  `esActivo` smallint DEFAULT NULL,
  `fechaRegistro` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rol`
--

LOCK TABLES `rol` WRITE;
/*!40000 ALTER TABLE `rol` DISABLE KEYS */;
INSERT INTO `rol` VALUES (1,'Administrador',1,'2024-05-31 15:38:16'),(2,'Empleado',1,'2024-05-31 15:38:16'),(3,'Supervisor',1,'2024-05-31 15:38:16'),(4,'Invitado',1,'2024-05-31 15:39:18');
/*!40000 ALTER TABLE `rol` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `etapa`
--

DROP TABLE IF EXISTS `etapa`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `etapa` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Codigo` varchar(5) NOT NULL,
  `Descripcion` varchar(100) NOT NULL,
  `Orden` int NOT NULL,
  `EstaActivo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_Etapa_Codigo` (`Codigo`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `etapa`
--

LOCK TABLES `etapa` WRITE;
/*!40000 ALTER TABLE `etapa` DISABLE KEYS */;
INSERT INTO `etapa` VALUES (1,'VIS','Visita',1,1),(2,'COT','Cotización',2,1),(3,'SEG','Seguimiento',3,1),(4,'PRE','Pre-Contrato',5,1),(5,'CON','Contrato',6,1),(6,'POR','Por Renovar',7,1),(8,'HIST','Histórico',99,0),(9,'ACE','Cotización Aceptada',4,1);
/*!40000 ALTER TABLE `etapa` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `permisosrol`
--

DROP TABLE IF EXISTS `permisosrol`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `permisosrol` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `secRol` int NOT NULL,
  `secUsuarioModifica` int NOT NULL,
  `fechaRegistro` datetime NOT NULL,
  `consultar` smallint NOT NULL,
  `modificar` smallint NOT NULL,
  `eliminar` smallint NOT NULL,
  `activo` smallint NOT NULL,
  PRIMARY KEY (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `permisosrol`
--

LOCK TABLES `permisosrol` WRITE;
/*!40000 ALTER TABLE `permisosrol` DISABLE KEYS */;
INSERT INTO `permisosrol` VALUES (1,1,1,'2025-03-17 00:00:00',1,1,1,1),(2,4,0,'0001-01-01 00:00:00',1,0,0,1),(3,3,0,'0001-01-01 00:00:00',0,1,0,1),(4,2,0,'0001-01-01 00:00:00',1,1,1,1);
/*!40000 ALTER TABLE `permisosrol` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rolpermiso`
--

DROP TABLE IF EXISTS `rolpermiso`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rolpermiso` (
  `SecRol` int NOT NULL,
  `IdPermiso` varchar(100) NOT NULL,
  PRIMARY KEY (`SecRol`,`IdPermiso`),
  KEY `fk_rolpermiso_permiso` (`IdPermiso`),
  CONSTRAINT `fk_rolpermiso_permiso` FOREIGN KEY (`IdPermiso`) REFERENCES `permiso` (`IdPermiso`),
  CONSTRAINT `fk_rolpermiso_rol` FOREIGN KEY (`SecRol`) REFERENCES `rol` (`secuencial`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rolpermiso`
--

LOCK TABLES `rolpermiso` WRITE;
/*!40000 ALTER TABLE `rolpermiso` DISABLE KEYS */;
INSERT INTO `rolpermiso` VALUES (1,'CREATE'),(1,'READ'),(2,'READ'),(1,'Roles.Administrar'),(1,'UPDATE'),(1,'VIEWMENU');
/*!40000 ALTER TABLE `rolpermiso` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `plantillacorreo`
--

DROP TABLE IF EXISTS `plantillacorreo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `plantillacorreo` (
  `SecPlantillaCorreo` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) NOT NULL,
  `Asunto` varchar(255) NOT NULL,
  `ContenidoHTML` text NOT NULL,
  `EstaActivo` smallint NOT NULL DEFAULT '1',
  `FechaRegistro` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`SecPlantillaCorreo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `plantillacorreo`
--

LOCK TABLES `plantillacorreo` WRITE;
/*!40000 ALTER TABLE `plantillacorreo` DISABLE KEYS */;
/*!40000 ALTER TABLE `plantillacorreo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tipodocumento`
--

DROP TABLE IF EXISTS `tipodocumento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tipodocumento` (
  `SecTipoDocumento` int NOT NULL AUTO_INCREMENT,
  `Codigo` varchar(50) NOT NULL,
  `Descripcion` varchar(255) NOT NULL,
  `EstaActivo` tinyint(1) NOT NULL DEFAULT '1',
  `FechaRegistro` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`SecTipoDocumento`),
  UNIQUE KEY `Codigo` (`Codigo`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tipodocumento`
--

LOCK TABLES `tipodocumento` WRITE;
/*!40000 ALTER TABLE `tipodocumento` DISABLE KEYS */;
INSERT INTO `tipodocumento` VALUES (1,'PRE-CONTRATO','Documento de Pre-Contrato',1,'2025-09-04 05:49:51'),(2,'CONTRATO','Documento de Contrato Final',1,'2025-09-04 05:49:51'),(3,'OFERTA','Documento de Oferta Comercial',1,'2025-09-04 05:49:51');
/*!40000 ALTER TABLE `tipodocumento` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `diccionarioparametro`
--

DROP TABLE IF EXISTS `diccionarioparametro`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `diccionarioparametro` (
  `Secuencial` int NOT NULL AUTO_INCREMENT,
  `Parametro` varchar(100) NOT NULL,
  `Descripcion` varchar(255) DEFAULT NULL,
  `EstaActivo` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Secuencial`),
  UNIQUE KEY `UQ_DiccionarioParametro_Parametro` (`Parametro`)
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `diccionarioparametro`
--

LOCK TABLES `diccionarioparametro` WRITE;
/*!40000 ALTER TABLE `diccionarioparametro` DISABLE KEYS */;
INSERT INTO `diccionarioparametro` VALUES (1,'{{Nombres Contrato}}','Nombre a quien se le va a generar el contrato',_binary ''),(3,'{{fecha_actual}}','La fecha del día en que se genera el documento.',_binary ''),(4,'{{hora_actual}}','La hora en que se genera el documento.',_binary ''),(5,'{{empresa_nombre}}','Nombre de tu empresa.',_binary ''),(6,'{{empresa_identificacion}}','RUC o identificación de tu empresa.',_binary ''),(7,'{{empresa_direccion}}','Dirección de tu empresa.',_binary ''),(8,'{{empresa_telefono}}','Teléfono de tu empresa.',_binary ''),(9,'{{empresa_correo}}','Correo electrónico de tu empresa.',_binary ''),(10,'{{cliente_nombre}}','Razón social o nombre de la constructora.',_binary ''),(11,'{{cliente_direccion}}','Dirección principal de la constructora.',_binary ''),(12,'{{cliente_telefono}}','Teléfono principal de la constructora.',_binary ''),(13,'{{cliente_correo}}','Correo principal de la constructora.',_binary ''),(14,'{{cliente_representante_legal}}','Nombre del administrador o representante de la constructora.',_binary ''),(15,'{{cliente_representante_telefono}}','Teléfono del administrador.',_binary ''),(16,'{{cliente_representante_correo}}','Correo del administrador.',_binary ''),(17,'{{contacto_nombre_completo}}','Nombres y apellidos del contacto principal asociado a la visita/cotización.',_binary ''),(18,'{{contacto_titulo}}','Título profesional del contacto (ej: Ing., Arq.).',_binary ''),(19,'{{contacto_correo}}','Correo del contacto.',_binary ''),(20,'{{contacto_telefono}}','Teléfono del contacto.',_binary ''),(21,'{{proyecto_nombre}}','Nombre del proyecto u obra.',_binary ''),(22,'{{proyecto_direccion}}','Dirección específica del proyecto.',_binary ''),(23,'{{proyecto_detalle}}','Descripción o detalles de la visita.',_binary ''),(24,'{{proyecto_provincia}}','Provincia donde se ubica el proyecto.',_binary ''),(25,'{{proyecto_canton}}','Cantón donde se ubica el proyecto.',_binary ''),(26,'{{cotizacion_numero}}','El número o secuencial de la cotización base.',_binary ''),(27,'{{cotizacion_subtotal}}','El valor subtotal de la cotización.',_binary ''),(28,'{{cotizacion_iva}}','El valor del IVA calculado.',_binary ''),(29,'{{cotizacion_total}}','El valor final (Total con Impuestos).',_binary ''),(30,'{{cotizacion_fecha_emision}}','La fecha de registro de la cotización.',_binary ''),(31,'{{tabla_detalle_cotizacion}}','Parámetro especial que se reemplazará por una tabla HTML con el detalle de la cotización.',_binary '');
/*!40000 ALTER TABLE `diccionarioparametro` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `empresa`
--

DROP TABLE IF EXISTS `empresa`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `empresa` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `urlLogo` varchar(500) DEFAULT NULL,
  `nombreLogo` varchar(100) DEFAULT NULL,
  `identificacion` varchar(15) DEFAULT NULL,
  `nombre` varchar(50) DEFAULT NULL,
  `correo` varchar(150) DEFAULT NULL,
  `direccion` varchar(250) DEFAULT NULL,
  `telefono` varchar(10) DEFAULT NULL,
  `codigoOperador` varchar(5) DEFAULT NULL,
  `estaActivo` smallint DEFAULT NULL,
  PRIMARY KEY (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `empresa`
--

LOCK TABLES `empresa` WRITE;
/*!40000 ALTER TABLE `empresa` DISABLE KEYS */;
INSERT INTO `empresa` VALUES (1,'https://firebasestorage.googleapis.com/v0/b/tecmein-c34ce.appspot.com/o/Imagenes_Logo%2F6ec8cbd962644c4ba1ae34b70b100b03.jpeg?alt=media&token=f052d5b4-9369-4c38-b43e-b0f6a0304fe0','6ec8cbd962644c4ba1ae34b70b100b03.jpeg','0603703646','Tecmein','correo@gmial.com','direccion de pruebasrr','023454810','EMP1',1),(3,'',NULL,'1720273372','pruebas2','correoemp@com','direccion depruebas 2','0932165478','EMP2',1),(4,'',NULL,'1801010792','pruebas3','emailpruebas3@gmail.com','direccion de pruebas mmm','0789654123','OPER3',1);
/*!40000 ALTER TABLE `empresa` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `formapago`
--

DROP TABLE IF EXISTS `formapago`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `formapago` (
  `SecFormaPago` int NOT NULL AUTO_INCREMENT,
  `Descripcion` varchar(100) NOT NULL,
  `EstaActivo` smallint NOT NULL DEFAULT '1',
  PRIMARY KEY (`SecFormaPago`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `formapago`
--

LOCK TABLES `formapago` WRITE;
/*!40000 ALTER TABLE `formapago` DISABLE KEYS */;
INSERT INTO `formapago` VALUES (1,'EFECTIVO',1);
/*!40000 ALTER TABLE `formapago` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `permiso`
--

DROP TABLE IF EXISTS `permiso`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `permiso` (
  `IdPermiso` varchar(100) NOT NULL,
  `Descripcion` varchar(255) NOT NULL,
  PRIMARY KEY (`IdPermiso`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `permiso`
--

LOCK TABLES `permiso` WRITE;
/*!40000 ALTER TABLE `permiso` DISABLE KEYS */;
INSERT INTO `permiso` VALUES ('CREATE','Permite crear nuevas entidades (registros)'),('DELETE','Permite eliminar entidades existentes'),('READ','Permite leer/ver la información y listas de entidades'),('Roles.Administrar','Permite acceso completo al módulo de administración de roles'),('UPDATE','Permite actualizar entidades existentes'),('VIEWMENU','Permite ver el elemento en el menú principal');
/*!40000 ALTER TABLE `permiso` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `provincia`
--

DROP TABLE IF EXISTS `provincia`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `provincia` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `codigo` varchar(5) DEFAULT NULL,
  `nombre` varchar(100) DEFAULT NULL,
  `estaActivo` smallint DEFAULT NULL,
  PRIMARY KEY (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `provincia`
--

LOCK TABLES `provincia` WRITE;
/*!40000 ALTER TABLE `provincia` DISABLE KEYS */;
INSERT INTO `provincia` VALUES (1,'01','AZUAY',1),(2,'02','BOLIVAR',1),(3,'03','CANIAR',1),(4,'04','CARCHI',1),(5,'05','COTOPAXI',1),(6,'06','CHIMBORAZO',1),(7,'07','EL ORO',1),(8,'08','ESMERALDAS',1),(9,'09','GUAYAS',1),(10,'10','IMBABURA',1),(11,'11','LOJA',1),(12,'12','LOS RIOS',1),(13,'13','MANABI',1),(14,'14','MORONA SANTIAGO',1),(15,'15','NAPO',1),(16,'16','PASTAZA',1),(17,'17','PICHINCHA',1),(18,'18','TUNGURAHUA',1),(19,'19','ZAMORA CHINCHIPE',1),(20,'20','GALAPAGOS',1),(21,'21','SUCUMBIOS',1),(22,'22','ORELLANA',1),(23,'23','SANTO DOMINGO DE LOS TSACHILAS',1),(24,'24','SANTA ELENA',1);
/*!40000 ALTER TABLE `provincia` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuario`
--

DROP TABLE IF EXISTS `usuario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuario` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(150) DEFAULT NULL,
  `correo` varchar(150) DEFAULT NULL,
  `telefono` varchar(50) DEFAULT NULL,
  `secRol` int DEFAULT NULL,
  `urlFoto` varchar(500) DEFAULT NULL,
  `nombreFoto` varchar(100) DEFAULT NULL,
  `clave` varchar(255) DEFAULT NULL,
  `esActivo` smallint DEFAULT NULL,
  `fechaRegistro` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`secuencial`),
  KEY `Fk_Rol_Usuario_idx` (`secRol`),
  CONSTRAINT `Fk_Rol_Usuario` FOREIGN KEY (`secRol`) REFERENCES `rol` (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuario`
--

LOCK TABLES `usuario` WRITE;
/*!40000 ALTER TABLE `usuario` DISABLE KEYS */;
INSERT INTO `usuario` VALUES (1,'Tecmein','codigo@example.com','0987135698',1,'https://firebasestorage.googleapis.com/v0/b/tecmein-c34ce.appspot.com/o/Imagenes_Usuario%2F5588c556c15b42b6af72f4f5d905fa8d.JPG?alt=media&token=c7a9bf9e-8d2d-4333-80af-cd70c0e1b9aa','','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3',1,'2024-05-31 15:50:10'),(3,'Byron Perrazo','byronperrazo@gmail.com','0987654321',2,'https://firebasestorage.googleapis.com/v0/b/tecmein-c34ce.appspot.com/o/Imagenes_Usuario%2F9205cbaffcef4fb88b54c53ca08786e9.JPG?alt=media&token=654b21c0-c681-4285-98f4-9c8d604e6a65','7e495a5f8f5940ebaad922791d5cacf1.JPG','8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92',1,'2024-06-05 08:38:44'),(4,'Byron o','akitushop@gmail.com','0987654989',4,'https://firebasestorage.googleapis.com/v0/b/tecmein-c34ce.appspot.com/o/Imagenes_Usuario%2F339cf49517654b18b78cac806137e0c0.JPG?alt=media&token=40f88642-77bd-4a67-8d7c-40fe7757ef2e','0-a3418db0.JPG','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3',1,'2024-06-05 09:14:13');
/*!40000 ALTER TABLE `usuario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rolmenu`
--

DROP TABLE IF EXISTS `rolmenu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rolmenu` (
  `secRol` int NOT NULL DEFAULT '0',
  `secMenu` int NOT NULL DEFAULT '0',
  `Actualizar` tinyint(1) NOT NULL DEFAULT '0',
  `Crear` tinyint(1) NOT NULL DEFAULT '0',
  `Eliminar` tinyint(1) NOT NULL DEFAULT '0',
  `Leer` tinyint(1) NOT NULL DEFAULT '0',
  `VerMenu` tinyint(1) NOT NULL DEFAULT '0',
  KEY `FK_Rol_Menu_idx` (`secRol`),
  KEY `FK_Menu_Rol_idx` (`secMenu`),
  CONSTRAINT `FK_Menu_Rol` FOREIGN KEY (`secMenu`) REFERENCES `menu` (`secuencial`),
  CONSTRAINT `FK_Rol_Menu` FOREIGN KEY (`secRol`) REFERENCES `rol` (`secuencial`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rolmenu`
--

LOCK TABLES `rolmenu` WRITE;
/*!40000 ALTER TABLE `rolmenu` DISABLE KEYS */;
INSERT INTO `rolmenu` VALUES (1,1,0,0,0,0,1),(1,2,0,0,0,0,1),(1,3,0,0,0,0,1),(1,4,0,0,0,0,1),(1,6,1,1,1,1,1),(1,7,1,1,1,1,1),(1,10,1,1,1,1,1),(1,12,1,1,1,1,1),(1,13,1,1,1,1,1),(1,15,1,1,1,1,1),(1,16,1,1,1,1,1),(1,17,0,0,0,0,1),(1,18,1,1,1,1,1),(1,19,1,1,1,1,1),(1,20,0,0,0,1,1),(1,21,1,1,1,1,1),(1,23,1,1,1,1,1),(1,24,1,1,1,1,1),(1,25,1,1,1,1,1),(1,27,1,1,1,1,1),(1,29,1,1,1,1,1),(1,31,1,1,1,1,1),(1,32,1,1,1,1,1),(1,41,1,1,1,1,1),(1,43,1,1,1,1,1),(1,44,1,1,1,1,1),(1,46,1,1,1,1,1),(1,49,1,1,1,1,1),(2,10,1,1,1,1,1),(2,12,1,1,1,1,1);
/*!40000 ALTER TABLE `rolmenu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `empresastorage`
--

DROP TABLE IF EXISTS `empresastorage`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `empresastorage` (
  `secEmpresa` int NOT NULL,
  `email` varchar(150) DEFAULT NULL,
  `clave` varchar(255) DEFAULT NULL,
  `ruta` varchar(255) DEFAULT NULL,
  `apiKey` varchar(255) DEFAULT NULL,
  `carpetaUsuario` varchar(255) DEFAULT NULL,
  `carpetaProducto` varchar(255) DEFAULT NULL,
  `carpetaLogo` varchar(255) DEFAULT NULL,
  `estaActivo` smallint DEFAULT NULL,
  PRIMARY KEY (`secEmpresa`),
  CONSTRAINT `empresastorage_ibfk_1` FOREIGN KEY (`secEmpresa`) REFERENCES `empresa` (`secuencial`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `empresastorage`
--

LOCK TABLES `empresastorage` WRITE;
/*!40000 ALTER TABLE `empresastorage` DISABLE KEYS */;
INSERT INTO `empresastorage` VALUES (1,'tecmeinstorage@tecmein.com','tecmeinstorage','tecmein-c34ce.appspot.com','AIzaSyBW4KLlZMKcHEcIezMdFFarcqLROhUs_g0','Imagenes_Usuario','Imagenes_Producto','Imagenes_Logo',1);
/*!40000 ALTER TABLE `empresastorage` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-11-04 13:08:23
