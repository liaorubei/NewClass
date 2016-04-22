
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