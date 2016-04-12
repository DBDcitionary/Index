using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Sc­hema;
using System.Data.Entity;
using System.Data.Sql;

namespace WebApplication1.Models
{
    public class qlist
    { 
        //Properties for Table Elements
        [Key]
        public string TBL_Name { get; set; }
        public string TBL_Description { get; set; }

        //Properties for Field Elements
        [Key]
        public string Field_Name { get; set; }
        public string Field_Description { get; set; }

        public PagedList.IPagedList<Database_Tbl> dblist { get; set; }
        public PagedList.IPagedList<Table_Tbl> tbllist { get; set; }
        public PagedList.IPagedList<Field_Tbl> fldlist { get; set; }

    }

    public class Database : Database_Tbl
    {

    }
}
