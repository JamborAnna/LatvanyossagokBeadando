﻿CREATE TABLE IF NOT EXISTS varosok(
	id INT PRIMARY KEY AUTO_INCREMENT,
	nev VARCHAR(1000) NOT NULL,
	lakossag INT NOT NULL
);

CREATE TABLE IF NOT EXISTS latvanyossagok(
	id INT PRIMARY KEY AUTO_INCREMENT,
	nev VARCHAR(1000) NOT NULL,
	leiras TEXT(100) NOT NULL,
	ar INT NOT NULL DEFAULT 0,
	varos_id INT NOT NULL,
	FOREIGN KEY(varos_id) 
	REFERENCES varosok(id)	
);