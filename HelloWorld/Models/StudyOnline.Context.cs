﻿//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace StudyOnline.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class StudyOnlineEntities : DbContext
    {
        public StudyOnlineEntities()
            : base("name=StudyOnlineEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Level> Level { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<UploadFile> UploadFile { get; set; }
        public DbSet<Folder> Folder { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<Group> Group { get; set; }
    }
}
