using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Sc­hema;
using System.Data.Entity;

namespace WebApplication1.Models
{
    [Table("Table_Tbl")]
    public class DBModel
    { 
        //Properties for Database Elements
        [Key]       
        public int DB_ID { get; set;  }
        public string DB_Name { get; set; }
        public string DB_Description { get; set; }
    }

    public class tbl
    {
        [Key]
        public int TBL_ID { get; set; }
        public string TBL_Name { get; set; }
        public string TBL_Description { get; set; }
        public int DB_ID { get; set; }
        public List<Database_Tbl> TableInfor { get; set; }
    }

    public class field
    {
        [Key]
        public int Field_ID { get; set; }
        public string Field_Name { get; set; }
        public string Field_Description { get; set; }
        public int TBL_ID { get; set; }
        public List<Table_Tbl> FieldInfor { get; set; }
    }
}
