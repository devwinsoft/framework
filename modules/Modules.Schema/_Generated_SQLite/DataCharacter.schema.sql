DROP TABLE IF EXISTS DataCharacter;
CREATE TABLE DataCharacter (
	unit_type TEXT NOT NULL PRIMARY KEY,
	show TEXT NOT NULL,
	name TEXT NOT NULL,
	items TEXT NOT NULL,
	stats TEXT NOT NULL,
	ability TEXT NOT NULL,
	nodes TEXT NOT NULL,
	unit_uid TEXT NOT NULL,
	specialCode TEXT NOT NULL,
	player_data TEXT NOT NULL
);
