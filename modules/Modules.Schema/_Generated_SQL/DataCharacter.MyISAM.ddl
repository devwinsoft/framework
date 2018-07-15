DROP TABLE IF EXISTS DataCharacter;
CREATE TABLE DataCharacter (
	`key` varchar(256) NOT NULL PRIMARY KEY,
	`data` varchar(2560) NOT NULL
 ) ENGINE=MyISAM DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
