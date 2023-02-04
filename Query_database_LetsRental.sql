/*
SQLyog Community v13.1.9 (64 bit)
MySQL - 10.4.22-MariaDB : Database - letsrental
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`letsrental` /*!40100 DEFAULT CHARACTER SET utf8mb4 */;

USE `letsrental`;

/*Table structure for table `admin` */

DROP TABLE IF EXISTS `admin`;

CREATE TABLE `admin` (
  `admin_id` int(11) NOT NULL AUTO_INCREMENT,
  `username_admin` varchar(100) DEFAULT NULL,
  `Admin_passwordHash` varbinary(256) DEFAULT NULL,
  `Admin_passwordSalt` varbinary(256) DEFAULT NULL,
  `Admin_name` varchar(100) DEFAULT NULL,
  `bpkb_image` varbinary(1000) DEFAULT NULL,
  `stnk_image` varbinary(1000) DEFAULT NULL,
  `fk_car_id` int(11) DEFAULT NULL,
  `Role` varchar(25) DEFAULT NULL,
  `active` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`admin_id`),
  KEY `fk_car_id` (`fk_car_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `car` */

DROP TABLE IF EXISTS `car`;

CREATE TABLE `car` (
  `car_id` int(11) NOT NULL AUTO_INCREMENT,
  `car_brand` varchar(100) DEFAULT NULL,
  `car_years` int(11) DEFAULT NULL,
  `car_rental_price` double DEFAULT NULL,
  `car_amount` int(11) DEFAULT NULL,
  `car_image` longtext DEFAULT NULL,
  `car_variant` varchar(100) DEFAULT NULL,
  `car_fuel` varchar(100) DEFAULT NULL,
  `fk_user_id` int(11) DEFAULT NULL,
  `fk_admin_id` int(11) DEFAULT NULL,
  `description` varchar(100) DEFAULT NULL,
  `keywords` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`car_id`),
  KEY `user_id` (`fk_user_id`),
  KEY `fk_admin_id` (`fk_admin_id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `cart` */

DROP TABLE IF EXISTS `cart`;

CREATE TABLE `cart` (
  `cart_id` int(11) NOT NULL AUTO_INCREMENT,
  `fk_car_id` int(11) DEFAULT NULL,
  `car_rental_days` int(11) DEFAULT NULL,
  `fk_user_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`cart_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `user` */

DROP TABLE IF EXISTS `user`;

CREATE TABLE `user` (
  `user_id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(100) DEFAULT NULL,
  `passwordHash` varbinary(256) DEFAULT NULL,
  `passwordSalt` varbinary(256) DEFAULT NULL,
  `name` varchar(100) DEFAULT NULL,
  `Role` varchar(25) DEFAULT NULL,
  `active` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
