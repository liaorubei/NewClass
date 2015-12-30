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

--创建主题,话题
create table Theme
(
   [Id]       int identity (1,1),
   [Name]     nvarchar(256) not null,
   constraint [PK_Theme] primary key (Id)
)

--创建问题
create table Question
(
   [Id]       int identity (1,1),
   [Name]     nvarchar(256) not null,
   [ThemeId]  int,
   constraint [PK_Question]       primary key (Id),
   constraint [FK_Question_Theme] foreign key (ThemeId) references [Theme] ([Id])
)

--通话记录
create table CallLog(
	[Id]         NVARCHAR (32) NOT NULL,
	[Source]     INT           NOT NULL,
	[Target]     INT           NOT NULL,
	[Start]      DATETIME,
	[Finish]	 DATETIME,
	CONSTRAINT [PK_CallLog]                PRIMARY KEY([Id]),
	CONSTRAINT [FK_CallLog_NimUser_Source] FOREIGN KEY([Source]) REFERENCES [NimUser]([Id]),
	CONSTRAINT [FK_CallLog_NimUser_Target] FOREIGN KEY([Target]) REFERENCES [NimUser]([Id])
)


--drop table nimuser
--drop table nimuserex
--drop table teacher
--drop table [group]
select * from NimUser order by enqueue
select * from NimUserEx

insert into nimuser(accid,token) values('bf09f7dd02e549f4a16af0cf8e9a5701');


select * from CallLog

