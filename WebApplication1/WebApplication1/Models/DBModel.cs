using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Sc­hema;
using System.Data.Entity;

namespace WebApplication1.Models
{
        public class qlist
        {
            //Properties for Database Elements
            [Key]
            public int DB_ID { get; set; }
            public string DB_Name { get; set; }
            public string DB_Description { get; set; }

            //Properties for Table Elements
            [Key]
            public int TBL_ID { get; set; }
            public string TBL_Name { get; set; }
            public string TBL_Description { get; set; }

            //Properties for Field Elements
            [Key]
            public int Field_ID { get; set; }
            public string Field_Name { get; set; }
            public string Field_Description { get; set; }
            
            //Properties for ObjectType
            public int ObjectTypeID { get; set; }
            public string Object_Type { get; set; }
            public string Object_Description { get; set; }

            public List<Database_Tbl> dblist { get; set; }
            public List<Table_Tbl> tbllist { get; set; }
            public List<Field_Tbl> fldlist { get; set; }
            public List<lk_ObjectType> lkO_Type { get; set; }
        }
}
