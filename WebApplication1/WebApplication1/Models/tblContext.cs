using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace WebApplication1.Models
{
    public class tblContext : DbContext
    {
            public DbSet<Database_Tbl> dbInfor { get; set; }
            public DbSet<Table_Tbl> tblInfor { get; set; }
            public DbSet<Field_Tbl> fldInfor { get; set; }
    }
}