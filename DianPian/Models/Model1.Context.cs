﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace DianPian.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class WoYaoXueEntities : DbContext
    {
        public WoYaoXueEntities()
            : base("name=WoYaoXueEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<Folder> Folder { get; set; }
        public virtual DbSet<Level> Level { get; set; }
        public virtual DbSet<UploadFile> UploadFile { get; set; }
    }
}
