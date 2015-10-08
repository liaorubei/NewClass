

--alter table Level add Sort int,Show int;

--alter table document add FolderId int
--alter table document add CONSTRAINT [FK_Document_Folder] FOREIGN KEY (FolderId) REFERENCES Folder(Id)

--alter table Folder add LevelId int ;
--alter table folder add constraint [FK_Folder_Level] foreign key (LevelId) references Level(Id)

--create table Folder(Id int primary key identity(1,1),Name nvarchar(50) not null)

--update document set folderid=null where id>450;

select * from Level

select id,title,folderid from document where id in(492,493,494)

select * from Folder