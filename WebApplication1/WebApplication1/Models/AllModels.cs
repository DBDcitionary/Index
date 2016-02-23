using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace WebApplication1.Models
{
    public class AllModels
    {
        public PagedList.IPagedList<Database_Tbl> dblist { get; set; }
        public PagedList.IPagedList<Table_Tbl> tbllist { get; set; }
        public PagedList.IPagedList<Field_Tbl> fldlist { get; set; }
    }
}