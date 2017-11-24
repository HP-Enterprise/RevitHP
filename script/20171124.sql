CREATE TABLE version(
	major tinyint not null,
	minor tinyint not null,
	fix   tinyint not null,
	tm    datetime not null, -- timestamp

	primary key(major, minor, fix)
);

INSERT INTO version(major, minor, fix, tm)
VALUES(1, 0, 0, datetime('now'));