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
    public class Search
    {
        public PagedList.IPagedList<Database_Tbl> dblist { get; set; }
        public PagedList.IPagedList<Table_Tbl> tbllist { get; set; }
        public PagedList.IPagedList<Field_Tbl> fldlist { get; set; }
    }
}
