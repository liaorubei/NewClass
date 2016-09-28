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
--alter table [NimUserEx] add [Coins] int
--alter table [NimUserEx] add [Score] float

--alter table [NimUserEx] add [School] nvarchar(1024);
--alter table [NimUserEx] add [Spoken] nvarchar(512);
--alter table [NimUserEx] add [Hobbies] nvarchar(512);



select * from [orders] order by createtime


----创建教师表
--CREATE TABLE [Teacher] 
--(
--    [Id]     int,
--    [Category]         INT              NOT NULL, --教师分类,如专职,兼职的
--    [IsOnline]         INT              ,		  --是否在线	
--	[IsAvailable]      INT              ,         --当前是否可以连接
--	[LastRefresh]      BIGINT           ,     --上次刷新在线状态的时间DateTime.Now.Ticks
--	[EnqueueTime]      BIGINT           ,     --上次入队的时间

--	CONSTRAINT [PK_Teacher]         PRIMARY KEY ([Id]),
--	CONSTRAINT [FK_Teacher_NimUser] FOREIGN KEY ([Id]) REFERENCES [NimUser] ([Id])
--);
--GO

----创建群组,群聊
--CREATE TABLE [Group]
--(
--	[Id]           NVARCHAR (32)    NOT NULL,
--	[Host]         int  ,
--	[Name]         NVARCHAR (64)    ,
--	[Time]         DATETIME		    ,
--	[Level]		   INT,		
--	[Theme]        NVARCHAR (1024)  ,
--	[Notice]       NVARCHAR (1024)  ,
--	[CreateDate]   DATETIME         ,
--	CONSTRAINT [PK_Group]         PRIMARY KEY (Id),
--	CONSTRAINT [FK_Group_NimUser] FOREIGN KEY (Host) REFERENCES [NimUser] ([Id])
--)

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


create table Level(
Id int identity(1,1),
Name nvarchar(256) not null,
Sort int not null,
Show int not null,--是否显示在客户端
ShowBrowser int , --是否在浏览器端显示
constraint PK_Level primary key(Id)
)
--alter table Level add Name nvarchar(256)
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
alter table  callLog add  Refresh datetime,Coins int
  alter table [dbo].[CallLog] add [IsBalance] int

  --添加时长,单价,学生平衡,教师平衡
  ALTER table [dbo].[CallLog] add Duration int,Price int,BalanceS int,BalanceT int

--学习记录的主题表
create table LogTheme(
[ChatId]     bigint ,
[ThemeId]    int,
CONSTRAINT [PK_LogTheme]         PRIMARY KEY([ChatId],[ThemeId]),
CONSTRAINT [FK_LogTheme_CallLog] FOREIGN KEY([ChatId]) REFERENCES [CallLog]([ChatId]),
CONSTRAINT [FK_LogTheme_ThemeId] FOREIGN KEY([ThemeId]) REFERENCES [Theme]([Id])
)

--反馈/投诉/建议表
create table Feedback(
[Id] int identity(1,1),
[Content] nvarchar(max),
[Contact] nvarchar(128),
[Createtime] datetime,
constraint [PK_Feedback] primary key ([Id])
)

--alter table [dbo].[Orders]
ALTER table [Orders] add  IsBalance int
--alter column [Quantity] decimal(10,2)
--alter column [Price] decimal(10,2)
--alter column [Amount] decimal(10,2)

select * from product

--充值价格条目表
create table Product(
Id     int identity(1,1),
[Coin] float,
[USD]  decimal(10,2),
[CNY]  decimal(10,2),
[Sort] int ,
[Enabled] int,
[Createtime] datetime,
constraint [PK_Product] primary key ([Id])
)
--alter table [dbo].[Product]
--alter column [USD] decimal(10,2)
--alter column [CNY] decimal(10,2)

--数据初始化
--insert into product(Coin,USD,CNY,Sort,[Enabled]) values(300,18.6,120.00,1,1)
--insert into product(Coin,USD,CNY,Sort,[Enabled]) values(500,31.00,200.00,2,1)
--insert into product(Coin,USD,CNY,Sort,[Enabled]) values(1000,62.00,400.00,3,1)
--insert into product(Coin,USD,CNY,Sort,[Enabled]) values(3000,186.00,1200.00,4,1)
--insert into product(Coin,USD,CNY,Sort,[Enabled]) values(5000,310.00,2000.00,5,1)
--insert into product(Coin,USD,CNY,Sort,[Enabled]) values(10000,620.00,4000.00,6,1)

--验证码表
create table AuthCode(
Id     NVARCHAR (32) NOT NULL,
[Code] NVARCHAR (32),
[Contact]  NVARCHAR (128),
[Createtime] datetime,
constraint [PK_AuthCode] primary key ([Id])
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



/****** Script for SelectTopNRows command from SSMS  ******/
SELECT * FROM [dbo].[NimUser] where category=1
--update [dbo].[NimUser] set [IsOnline]=1,[IsEnable]=1,[Refresh]='2016-01-26',[Enqueue]='2016-01-26',[System]=1 where category=1


--//20160726 重新添加刷新功能,这次加入了刷新的是哪种系统,哪个设备,并更改了数据库的刷新与入队字段的数据类型
--update [dbo].[NimUser] set [Refresh]=null
--alter table [dbo].[NimUser] alter column [Refresh] datetime

--update [dbo].[NimUser] set [Enqueue]=null
--alter table [dbo].[NimUser] alter column [Enqueue] datetime

--alter table [dbo].[NimUser] add [System] int           --手机系统,安卓为1,苹果为2,其它为0
--alter table [dbo].[NimUser] add [Device] nvarchar(128)  
--alter table [dbo].[NimUser] alter column [Device] nvarchar(128)

--//创建视图
--drop view View_User
--CREATE VIEW View_User as 
--select N.Id,N.Accid,N.Username,N.Category,N.IsOnline,N.IsEnable,N.IsActive,N.CreateDate,E.Email,E.Name as Nickname, E.Icon as Avatar,E.Country,E.Coins 
--from [dbo].[NimUser] as N inner join [dbo].[NimUserEx] as E on N.Id=E.Id

--//添加教师审核同步数据字段
--alter table [dbo].[Teacherreginfo] add IsSync int 

--20160808 添加教材分类,教材文件夹要求有封面显示功能
alter table [dbo].[Folder] add [Sort] int,[Cover] nvarchar(256)
alter table [dbo].[Level] add [HasCover] int

--文件夹要求有嵌套功能
alter table Folder add ParentId int, constraint FK_Folder_Folder foreign key (ParentId) references Folder(Id)

--//创建文档视图,
--Drop View View_Document
--Create View View_Document as 
--select L.Id as LevelId,L.Name as LevelName,L.[ShowBrowser],F.Id as FolderId,F.Name as  FolderName,D.Id as DocumentId,D.[Title],D.[SoundPath],D.[AuditCase],D.[AuditDate] from
--Level as L inner join Folder as F on L.Id=F.LevelId inner join [dbo].[Document] as D on F.Id=D.FolderId

--20160815由于加入课本,生词,阅读等原因,加之审核时间不确定,因此要求给文档加入排序功能1.1,1.2,1.3
alter table document add [Sort] float

--DROP TABLE [dbo].[Member_Folder]
--DROP TABLE [dbo].[Member_User]
--DROP TABLE [dbo].[Member]

--会员表
CREATE TABLE Member(
Id      varchar(32),
Name   nvarchar(32),
[Month]  int,
constraint PK_Member primary key ([Id])
)

--会员文件夹表
CREATE TABLE Member_Folder(
MemberId varchar(32),
FolderId int,
CONSTRAINT PK_Member_Folder        primary key ([MemberId],[FolderId]),
CONSTRAINT FK_Member_Folder_Member foreign key ([MemberId]) references Member (Id),
CONSTRAINT FK_Member_Folder_Folder foreign key ([FolderId]) references Folder (Id)
)

--会员用户表
CREATE TABLE Member_User(
MemberId varchar(32),
UserId int,
[From] datetime,
[To]   datetime,
CONSTRAINT PK_Member_User         PRIMARY KEY (MemberId,UserId),
CONSTRAINT FK_Member_User_Member  FOREIGN KEY (MemberId) REFERENCES Member  (Id),
CONSTRAINT FK_Member_User_User	  FOREIGN KEY (UserId)   REFERENCES NimUser (Id)        
)



--DROP   VIEW View_FolderWithLevel
--CREATE VIEW View_FolderWithLevel as 
SELECT F.Id as FolderId,F.Name as FolderName,F.Sort as FolderSort,F.Cover as FolderCover,L.[Id] AS LevelId,L.[Name] AS LevelName,L.Sort as LevelSort,L.Show as LevelShow,L.[ShowBrowser],L.[ShowCover] 
FROM [Folder] AS F LEFT JOIN [Level] AS L ON F.[LevelId]=L.[Id]
Order by L.Sort ,F.Sort,F.Name



--DROP   VIEW View_Folder_Member_User
--CREATE VIEW View_Folder_Member_User AS 
SELECT 
[Member_Folder].[MemberId],
[Member_Folder].[FolderId],
[Member_User].[UserId],
[Member_User].[From],
[Member_User].[To]
FROM [Member_Folder] INNER JOIN [Member_User] 
ON   [Member_Folder].[MemberId]=[Member_User].[MemberId]


--在查询学生或教师学习记录时会用到
--DROP   VIEW View_Chat_User
--CREATE VIEW View_Chat_user AS
SELECT
C.[Id],
C.[Source],
N1.Name AS Student,
C.[Target],
N2.Name AS Teacher,
C.[Start],
C.[Finish],
C.[ChatId],
C.[ChatType],
C.[Coins],
C.[Duration],
C.[BalanceS]
FROM      [CallLog] C
LEFT JOIN [NimUserEx] N1 ON C.[Source]=N1.Id
LEFT JOIN [NimUserEx] N2 ON C.[Target]=N2.Id


/****** Script for 创建一个方便给会员添加用户,或者用户添加会员功能的视图,主要是用在管理系统会员相关的模块  ******/
--DROP   VIEW View_UserLeftJoinMember
--CREATE VIEW View_UserLeftJoinMember as
(
SELECT N.[Id]
 ,N.[Username]
 ,N.[CreateDate]
 ,N.[Category]
 ,N.[IsOnline]
 ,N.[IsActive]
 ,N.[IsEnable]
 ,E.Name as Nickname
 ,E.[Email]
 ,E.[Birth]
 ,E.[Mobile]
 ,M.[MemberId]
 ,M.[From]
 ,M.[To]
FROM       [NimUser] as N 
INNER JOIN [NimUserEx] as E on N.Id=E.Id
LEFT  JOIN [Member_User] as M  on N.Id=M.UserId
) 


