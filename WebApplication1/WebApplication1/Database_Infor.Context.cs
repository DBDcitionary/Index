﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DB_DictionaryContext : DbContext
    {
        public DB_DictionaryContext()
            : base("name=DB_DictionaryContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Database_Tbl> Database_Tbl { get; set; }
        public virtual DbSet<Field_Tbl> Field_Tbl { get; set; }
        public virtual DbSet<Table_Tbl> Table_Tbl { get; set; }
        public virtual DbSet<User_Role_Tbl> User_Role_Tbl { get; set; }
        public virtual DbSet<User_Tbl> User_Tbl { get; set; }
    }
}