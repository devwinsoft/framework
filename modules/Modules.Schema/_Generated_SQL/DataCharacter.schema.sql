DROP TABLE IF EXISTS DataCharacter;
CREATE TABLE DataCharacter (
	`key` varchar(256) NOT NULL PRIMARY KEY,
	`data` varchar(4096) NOT NULL
 ) DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
