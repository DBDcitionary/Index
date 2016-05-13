using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class tableUpdate
    {
        public DB_DictionaryContext context { get; set; }
        public tableUpdate()
        {
        }

        public IList<Table_Tbl> updatetable(string name, string DBID, string FKID, string TBLDescrip)
        {
            context = new DB_DictionaryContext();
            int id = int.Parse(DBID);
            var desc = context.Table_Tbl.FirstOrDefault(a => a.TBL_Name == name && a.DB_ID == id && a.FK_Tableid == FKID);
            var tblmodel = new DB_DictionaryContext().Table_Tbl.ToList();
            var updateDate = DateTime.Now;
            using (context)
            {
                if(TBLDescrip != null)
                {
                    if (desc != null)
                    {
                        desc.TBL_Description = TBLDescrip;
                        desc.UpdatedDate = updateDate;
                        context.SaveChanges();
                    }
                } 
            }
            return tblmodel;

        }
    }
}