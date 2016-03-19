--创建云信关联用户表
CREATE TABLE [NimUser] 
(
	[Id]         INT IDENTITY(1,1),
	[Accid]      NVARCHAR (32)     NOT NULL,
	[Token]      NVARCHAR (32)     NOT NULL,
	[Username]   NVARCHAR (128)    NOT NULL,
	[Password]   NVARCHAR (128)    NOT NULL,
	
	[Category]   int,
	[IsOnline]	 int,
	[IsActive]	 int,
	[IsEnable]	 int,
	[Refresh]	 bigint,
	[Enqueue]	 bigint,
	[CreateDate] DATETIME,

	CONSTRAINT [PK_NimUser] PRIMARY KEY ([Id])
);
GO
CREATE UNIQUE NONCLUSTERED INDEX [Index_Accid] ON [NimUser]([Accid] ASC);
GO
CREATE UNIQUE NONCLUSTERED INDEX [Index_Username] ON [NimUser]([Username] ASC);
GO

CREATE TABLE [NimUserEx] 
(
	[Id]     int,
	[Name]	 NVARCHAR (64) ,
	[Icon]	 NVARCHAR (256),
	[Sign]	 NVARCHAR (256),
	[Email]	 NVARCHAR (64) ,
	[Birth]	 DATETIME      ,
	[Mobile] NVARCHAR (32) ,
	[Gender] INT		   ,
	[Ex]     NVARCHAR (512),

	CONSTRAINT [PK_NimUserEx]         PRIMARY KEY ([Id]),
	CONSTRAINT [FK_NimUserEx_NimUser] FOREIGN KEY ([Id]) REFERENCES [NimUser] ([Id])
);
GO
--alter table [NimUserEx] add [Country] nvarchar(256);
--alter table [NimUserEx] add [Language] nvarchar(256);
--alter table [NimUserEx] add [Job] nvarchar(256);
--alter table [NimUserEx] add [About] nvarchar(1024);
--alter table [NimUserEx] add [Voice] nvarchar(1024);
select * from [NimUserEx]



--创建教师表
CREATE TABLE [Teacher] 
(
    [Id]     int,
    [Category]         INT              NOT NULL, --教师分类,如专职,兼职的
    [IsOnline]         INT              ,		  --是否在线	
	[IsAvailable]      INT              ,         --当前是否可以连接
	[LastRefresh]      BIGINT           ,     --上次刷新在线状态的时间DateTime.Now.Ticks
	[EnqueueTime]      BIGINT           ,     --上次入队的时间

	CONSTRAINT [PK_Teacher]         PRIMARY KEY ([Id]),
	CONSTRAINT [FK_Teacher_NimUser] FOREIGN KEY ([Id]) REFERENCES [NimUser] ([Id])
);
GO

--创建群组,群聊
CREATE TABLE [Group]
(
	[Id]           NVARCHAR (32)    NOT NULL,
	[Host]         int  ,
	[Name]         NVARCHAR (64)    ,
	[Time]         DATETIME		    ,
	[Level]		   INT,		
	[Theme]        NVARCHAR (1024)  ,
	[Notice]       NVARCHAR (1024)  ,
	[CreateDate]   DATETIME         ,
	CONSTRAINT [PK_Group]         PRIMARY KEY (Id),
	CONSTRAINT [FK_Group_NimUser] FOREIGN KEY (Host) REFERENCES [NimUser] ([Id])
)

--创建汉语等级表
create table HsLevel(
	[Id] int identity (1,1),
	[Name] nvarchar(256) not null,
	constraint [PK_HsLevel] primary key ([Id])
);

--创建主题,话题
create table Theme
(
   [Id]           int identity (1,1),
   [Name]         nvarchar(256) not null,
   [HskLevelId]   int,
   constraint     [PK_Theme] primary key (Id),
   constraint     [FK_Theme_HskLevel] foreign key  ([HskLevelId]) references [HskLevel] ([Id])
)
--alter table theme add HsLevelId int ;
--alter table theme add constraint [FK_Theme_HsLevel] foreign key  ([HsLevelId]) references [HsLevel] ([Id])
--alter table Theme drop constraint FK_Theme_HskLevel
--alter table Theme drop column HskLevelId
--drop table HskLevel
--select * from Theme

--创建问题
create table Question
(
   [Id]       int identity (1,1),
   [Name]     nvarchar(256) not null,
   [ThemeId]  int,
   constraint [PK_Question]       primary key (Id),
   constraint [FK_Question_Theme] foreign key (ThemeId) references [Theme] ([Id])
)

--alter table Level add Name varchar(256)
--update Level set Name=LevelName
select * from Level

--通话记录
create table CallLog(
	[Id]         NVARCHAR (32) NOT NULL,
	[ChatId]     bigint ,
	[ChatType]   int,
	[Source]     INT           NOT NULL,
	[Target]     INT           NOT NULL,
	[Start]      DATETIME,
	[Finish]	 DATETIME,
	[Score]	     int,
	CONSTRAINT [PK_CallLog]                PRIMARY KEY([Id]),
	CONSTRAINT [FK_CallLog_NimUser_Source] FOREIGN KEY([Source]) REFERENCES [NimUser]([Id]),
	CONSTRAINT [FK_CallLog_NimUser_Target] FOREIGN KEY([Target]) REFERENCES [NimUser]([Id])
)
CREATE UNIQUE NONCLUSTERED INDEX [Index_CallLog_ChatId] ON [CallLog]([ChatId] ASC);


--学习记录的主题表
create table LogTheme(
[ChatId]     bigint ,
[ThemeId]    int,
CONSTRAINT [PK_LogTheme]         PRIMARY KEY([ChatId],[ThemeId]),
CONSTRAINT [FK_LogTheme_CallLog] FOREIGN KEY([ChatId]) REFERENCES [CallLog]([ChatId]),
CONSTRAINT [FK_LogTheme_ThemeId] FOREIGN KEY([ThemeId]) REFERENCES [Theme]([Id])
)

--安卓管理
CREATE TABLE [Android]
(
	[Id]             INT IDENTITY(1,1),
	[VersionType]    INT              ,
	[VersionName]    NVARCHAR (1024)  ,
	[PackageSize]    INT		      ,
	[UpgradeInfo]    NVARCHAR (1024)  ,		
	[PackagePath]	 NVARCHAR (1024)  ,	
	[CreateDate]     DATETIME         ,
	CONSTRAINT [PK_Android]  PRIMARY KEY (Id)
)

select * from android where versionType=0 order by createdate desc


--drop table nimuser
--drop table nimuserex
--drop table teacher
--drop table [group]
--insert into nimuser(accid,token) values('bf09f7dd02e549f4a16af0cf8e9a5701');

select * from NimUser where Accid='25f1ccdee9cd4f37b4c177c271076477' order by enqueue 
select * from NimUserEx
select * from nimuser order by createdate
select * from CallLog where ChatId in (select ChatId from LogTheme) order by start,ChatId
select * from theme
select * from question

--delete from CallLog where ChatId is null
select * from LogTheme where ChatId =6236230176636676750
select * from CallLog where source=9 order by Start 
select * from nimuser order by username
select * from android order by createdate
select * from theme

UPDATE Theme SET HsLevelId=null where HsLevelId=2