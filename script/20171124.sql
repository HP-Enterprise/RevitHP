-- 版本控制
CREATE TABLE version(
	script varchar not null primary key,
	md5    varchar not null, -- SQL的MD5值
	tm     datetime not null -- timestamp
);

-- 一个树状的层级结构
CREATE TABLE catalog(
	id     int not null primary key,
	name   varchar not null,
	parent smallint not NULL, -- parent id, 0 means top level
	newname nvarchar(10) default(''),--修改name
	NameID INT default(-1),--操作人ID
	identifying INT null,--状态标识
	audit int default(0) --是否审核
);

INSERT INTO catalog(id,name,parent) VALUES
(1, '族库', 0),
    (2, '电气专业', 1),
	    (21, '市电供配电系统', 2),
		(22, '应急柴油发电系统', 2),
		(23, 'UPS系统', 2),
		(24, '接地系统', 2),
		(25, '其他', 2),
    (3, '暖通专业', 1),
	    (31, '冷水机组', 3),
		(32, '冷水系统', 3),
		(33, '机房末端精密空调及盘管', 3),
		(34, '其他', 3),
	(4, '给排水及消防专业', 1),
	    (41, '给排水系统', 4),
		(42, '消防水系统', 4),
		(43, '消防报警及联动系统', 4),
		(44, '极早期消防报警系统', 4),
		(45, '气体灭火系统', 4),
		(46, '消防排烟系统', 4),
		(47, '建筑电气防火系统', 4),
		(48, '其他', 4),
	(5, '弱电专业', 1),
	    (51, '视频监控系统', 5),
		(52, '门禁系统', 5),
		(53, '出入口管理设备', 5),
		(54, '入侵报警系统', 5),
		(55, '电力监控系统', 5),
		(56, '柴油机供油监控系统', 5),
		(57, '机房环境系统', 5),
		(58, '其他', 5),
	(6, '建筑结构及装修专业', 1);

CREATE TABLE Model
(
id int not null primary key,
mod_name varchar not null,
mod_size varchar not null,
catalogid int not null
);
INSERT INTO Model(id,mod_name,mod_size,catalogid) VALUES
(1,'ff64011aec56c57954b751c7044a1abc','250165K',21),
(2,'ff64011aec56c57954b751c7044a1abc','250165K',22)

--INSERT INTO familymessage values('25','UPS-施耐德','Upsa系统','产品族','2017-1-1','11')
--Delete  familymessage
--INSERT INTO familymessage(id,fm_name,fm_classify,fm_fineness,fm_uploadtime,fm_versionnumber) VALUES
--(25,'UPS-施耐德','Upsa系统','产品族','2017-1-1',11),
--(55,'UPs-施耐庵','Upsaa系统','产品族','2017-1-1',12),
--(33,'UPS-吖','Upsaaa系统','产品族','2017-1-1',30)
--INSERT INTO familymessage(id,fm_name,fm_classify,fm_fineness,fm_uploadtime,fm_versionnumber) VALUES(112,'adfw','awfaw','awfawf','2017-7-7',2)









