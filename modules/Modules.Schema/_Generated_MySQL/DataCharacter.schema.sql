DROP TABLE IF EXISTS DataCharacter;
CREATE TABLE DataCharacter (
	`unit_type` varchar(50) NOT NULL PRIMARY KEY,
	`show` varchar(50) NOT NULL,
	`name` varchar(50) NOT NULL,
	`items` varchar(50) NOT NULL,
	`stats` varchar(50) NOT NULL,
	`ability` varchar(50) NOT NULL,
	`nodes` varchar(50) NOT NULL,
	`unit_uid` varchar(50) NOT NULL,
	`specialCode` varchar(50) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
