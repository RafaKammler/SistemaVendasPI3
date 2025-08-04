-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)
--
-- Host: localhost    Database: projetointegrador3
-- ------------------------------------------------------
-- Server version	9.2.0

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
-- Table structure for table `carrinho_item`
--

DROP TABLE IF EXISTS `carrinho_item`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `carrinho_item` (
  `CarrinhoItemID` int NOT NULL AUTO_INCREMENT,
  `ClienteID` int NOT NULL,
  `ProdutoID` int NOT NULL,
  `Quantidade` int NOT NULL,
  `DataAdicionado` datetime NOT NULL,
  PRIMARY KEY (`CarrinhoItemID`),
  UNIQUE KEY `UK_cliente_produto` (`ClienteID`,`ProdutoID`),
  KEY `ProdutoID` (`ProdutoID`),
  CONSTRAINT `carrinho_item_ibfk_1` FOREIGN KEY (`ClienteID`) REFERENCES `cliente` (`ClienteID`),
  CONSTRAINT `carrinho_item_ibfk_2` FOREIGN KEY (`ProdutoID`) REFERENCES `produto` (`ProdutoID`)
) ENGINE=InnoDB AUTO_INCREMENT=277 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cidade`
--

DROP TABLE IF EXISTS `cidade`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cidade` (
  `CidadeID` int NOT NULL AUTO_INCREMENT,
  `CidadeNome` varchar(100) NOT NULL,
  `CidadeCEP` varchar(20) DEFAULT NULL,
  `CidadeUF` char(2) DEFAULT NULL,
  PRIMARY KEY (`CidadeID`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cliente`
--

DROP TABLE IF EXISTS `cliente`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cliente` (
  `ClienteID` int NOT NULL AUTO_INCREMENT,
  `CidadeID` int DEFAULT NULL,
  `ClienteNome` varchar(100) NOT NULL,
  `ClienteCPF` varchar(14) DEFAULT NULL,
  `Senha` varchar(255) NOT NULL,
  `ClienteCep` varchar(20) DEFAULT NULL,
  `ClienteDataNascimento` date DEFAULT NULL,
  `ClienteTelefone` varchar(20) DEFAULT NULL,
  `ClienteSexo` char(1) DEFAULT NULL,
  PRIMARY KEY (`ClienteID`),
  UNIQUE KEY `ClienteCPF` (`ClienteCPF`),
  KEY `CidadeID` (`CidadeID`),
  CONSTRAINT `cliente_ibfk_1` FOREIGN KEY (`CidadeID`) REFERENCES `cidade` (`CidadeID`)
) ENGINE=InnoDB AUTO_INCREMENT=30 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `comentario`
--

DROP TABLE IF EXISTS `comentario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `comentario` (
  `ComentarioID` int NOT NULL AUTO_INCREMENT,
  `ProdutoID` int NOT NULL,
  `ClienteID` int NOT NULL,
  `ComentarioTexto` text NOT NULL,
  `DataComentario` datetime NOT NULL,
  PRIMARY KEY (`ComentarioID`),
  KEY `ProdutoID` (`ProdutoID`),
  KEY `ClienteID` (`ClienteID`),
  CONSTRAINT `comentario_ibfk_1` FOREIGN KEY (`ProdutoID`) REFERENCES `produto` (`ProdutoID`),
  CONSTRAINT `comentario_ibfk_2` FOREIGN KEY (`ClienteID`) REFERENCES `cliente` (`ClienteID`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fornecedor`
--

DROP TABLE IF EXISTS `fornecedor`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `fornecedor` (
  `FornecedorID` int NOT NULL AUTO_INCREMENT,
  `CidadeID` int DEFAULT NULL,
  `FornecedorNome` varchar(100) NOT NULL,
  `FornecedorCNPJ` varchar(18) DEFAULT NULL,
  `Senha` varchar(255) NOT NULL,
  `FornecedorCEP` varchar(20) DEFAULT NULL,
  `FornecedorEmail` varchar(100) DEFAULT NULL,
  `FornecedorTelefone` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`FornecedorID`),
  UNIQUE KEY `FornecedorCNPJ` (`FornecedorCNPJ`),
  KEY `CidadeID` (`CidadeID`),
  CONSTRAINT `fornecedor_ibfk_1` FOREIGN KEY (`CidadeID`) REFERENCES `cidade` (`CidadeID`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pedido`
--

DROP TABLE IF EXISTS `pedido`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pedido` (
  `PedidoID` int NOT NULL AUTO_INCREMENT,
  `FornecedorID` int DEFAULT NULL,
  `ClienteID` int DEFAULT NULL,
  `VendaData` date DEFAULT NULL,
  `VendaValor` decimal(10,2) DEFAULT NULL,
  `DataPedido` date DEFAULT NULL,
  `HoraPedido` time DEFAULT NULL,
  PRIMARY KEY (`PedidoID`),
  KEY `FornecedorID` (`FornecedorID`),
  KEY `ClienteID` (`ClienteID`),
  CONSTRAINT `pedido_ibfk_1` FOREIGN KEY (`FornecedorID`) REFERENCES `fornecedor` (`FornecedorID`),
  CONSTRAINT `pedido_ibfk_2` FOREIGN KEY (`ClienteID`) REFERENCES `cliente` (`ClienteID`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pedidoxproduto`
--

DROP TABLE IF EXISTS `pedidoxproduto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pedidoxproduto` (
  `PedidoID` int NOT NULL,
  `ProdutoID` int NOT NULL,
  `Quantidade` int DEFAULT NULL,
  PRIMARY KEY (`PedidoID`,`ProdutoID`),
  KEY `ProdutoID` (`ProdutoID`),
  CONSTRAINT `pedidoxproduto_ibfk_1` FOREIGN KEY (`PedidoID`) REFERENCES `pedido` (`PedidoID`),
  CONSTRAINT `pedidoxproduto_ibfk_2` FOREIGN KEY (`ProdutoID`) REFERENCES `produto` (`ProdutoID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `produto`
--

DROP TABLE IF EXISTS `produto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `produto` (
  `ProdutoID` int NOT NULL AUTO_INCREMENT,
  `FornecedorID` int DEFAULT NULL,
  `Preco` decimal(10,2) DEFAULT NULL,
  `Descricao` text,
  `Imagem` varchar(255) DEFAULT NULL,
  `Nome` varchar(255) DEFAULT NULL,
  `Estoque` int DEFAULT '0',
  PRIMARY KEY (`ProdutoID`),
  KEY `FornecedorID` (`FornecedorID`),
  CONSTRAINT `produto_ibfk_1` FOREIGN KEY (`FornecedorID`) REFERENCES `fornecedor` (`FornecedorID`)
) ENGINE=InnoDB AUTO_INCREMENT=117 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping routines for database 'projetointegrador3'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-08-04 19:40:12
