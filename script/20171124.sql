CREATE TABLE version(
	script varchar not null primary key,
	md5    varchar not null,
	tm     datetime not null -- timestamp
);
