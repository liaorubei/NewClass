
--创建用户表
CREATE TABLE [User] 
(
    [Id]               INT IDENTITY(1,1) NOT NULL,
	[NickName]         NVARCHAR (256)    NOT NULL,
    [UserName]         NVARCHAR (256)    NOT NULL,
    [Password]         NVARCHAR (256)    NOT NULL,
    [Phone]			   NVARCHAR (256)    NOT NULL,
	[Email]			   NVARCHAR (256)    NOT NULL,
	[IsOnline]         INT               NOT NULL,
	[CreateDate]       DATETIME          NOT NULL
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [User]([UserName] ASC);
GO

ALTER TABLE [User] ADD CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

--创建角色表
CREATE TABLE [Role] 
(
    [Id]               INT IDENTITY(1,1) NOT NULL,
    [RoleName]         NVARCHAR (256)    NOT NULL
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [Role]([RoleName] ASC);
GO

ALTER TABLE [Role] ADD CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

--创建用户角色表
CREATE TABLE [UserRole] (
    [UserId] INT NOT NULL,
    [RoleId] INT NOT NULL,
    CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_UserRole_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRole_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Role] ([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_UserId] ON [UserRole]([UserId] ASC);
GO 

CREATE NONCLUSTERED INDEX [IX_RoleId] ON [UserRole]([RoleId] ASC);
GO

--创建菜单表
--创建菜单表
CREATE TABLE [Menu]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[Name] NVARCHAR (256)    NOT NULL,
	[ParentId] int ,
	[Order] int,
	constraint [PK_Menu] primary key (Id),
	constraint [FK_Menu_MenuId] FOREIGN KEY ([ParentId]) REFERENCES [Menu] ([Id])
)

  Create table [Defect](
  [Id] int identity(1,1),
  [Name] nvarchar(256),
  [Desc] nvarchar(1024),
  [State] int,
  [Serious] int,

  CreateTime Datetime,
  Constraint PK_Defect primary key (Id)
  )


  /****** 2016-08-01 按要求添加后台权限管理系统  ******/
create table X_User(
[Id] char(32) not null,
[Username] varchar(128) not null UNIQUE,
[Password] varchar(128) not null,
[Nickname] nvarchar(128),
[Truename] nvarchar(128),
[IsActive] int not null,
[CreateDate] datetime not null,
constraint PK_X_User primary key (Id)
)

create table X_Role(
[Id] char(32) not null,
[Name] nvarchar(128),
constraint PK_X_Role primary key (Id)
)

Create table X_Menu_X_Role(
Id_Menu int,
Id_Role char(32),
constraint PK_X_Menu_X_Role primary key (Id_Menu,Id_Role),
Constraint FK_X_Menu_X_Role_X_Menu FOREIGN KEY ([Id_Menu]) REFERENCES [X_Menu] ([Id]),
constraint FK_X_Menu_X_Role_X_Role FOREIGN KEY ([Id_Role]) REFERENCES [X_Role] ([Id])
)

Create table X_Role_X_User(
[Id_Role] char(32),
[Id_User] char(32),
CONSTRAINT PK_X_Role_X_User primary key ([Id_Role],[Id_User]),
CONSTRAINT FK_X_Role_X_User_X_Role FOREIGN KEY ([Id_Role]) REFERENCES [X_Role] ([Id]),
CONSTRAINT FK_X_Role_X_User_X_User FOREIGN KEY ([Id_User]) REFERENCES [X_User] ([Id])
)

select * from [dbo].[X_User]
select * from [dbo].[X_Menu] order by parentId,[order]
select * from [dbo].[X_Role]

--修改主题和问题表,都添加一个序号的字段,用以排序,
alter table [dbo].[Theme] add Sort int
alter table [dbo].[Theme] add IsDelete int 
alter table [dbo].[Question] add Sort int 