

--alter table Level add Sort int,Show int;
--alter table Level add ShowBrowser int; --20151009添加是否在浏览器端显示的字段

--alter table document add FolderId int
--alter table document add CONSTRAINT [FK_Document_Folder] FOREIGN KEY (FolderId) REFERENCES Folder(Id)
--ALTER TABLE Document add AuditDate datetime, AuditCase int;
--update Document set AddDate=AuditDate; 
--update document set auditcase=2;
--create table Folder(Id int primary key identity(1,1),Name nvarchar(50) not null)
--alter table Folder add LevelId int ;
--alter table folder add constraint [FK_Folder_Level] foreign key (LevelId) references Level(Id)


--update document set folderid=null where id>450;
--update Level set ShowBrowser=1
--update Document set LevelId=(select F.LevelId from Folder as F where F.Id=Document.FolderId)

select * from Level

select id,title,folderid from document where id in(492,493,494)
SELECT * FROM Document order by AddDate desc

select * from Folder

--alter table [User] add [NickName] nvarchar(256)
select * from [user]


--创建用户表
CREATE TABLE [Customer] 
(
    [AccId]            NVARCHAR (32)    NOT NULL,
    [Account]          NVARCHAR (64)    NOT NULL,
    [Password]         NVARCHAR (64)    NOT NULL,
	[NickName]         NVARCHAR (64)    ,
    [Phone]			   NVARCHAR (256)   ,
	[Email]			   NVARCHAR (1024)  ,
	[Icon]			   NVARCHAR (1024)  ,
	[Gender]           INT              ,
	[Birthday]         DATETIME         ,
	[IsOnline]         INT              ,
	[CreateDate]       DATETIME         
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [AccountIndex] ON [Customer]([Account] ASC);
GO

ALTER TABLE [Customer] ADD CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([AccId] ASC);
GO


