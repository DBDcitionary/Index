using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace WebApplication1.Models
{
    public class UpdateFieldModel
    {
        //string connectionstring = WebConfigurationManager.ConnectionStrings["DB_Dictionary"].ToString();
        public DB_DictionaryContext Context_ { get; set;}

        public UpdateFieldModel()
        {

        }

        public IList<Field_Tbl> UpdateField(string Descript, string DBName, string tblname)
        {
            Context_ = new DB_DictionaryContext();
            var id = Context_.Field_Tbl.Where(a=>a.DBName == DBName && a.TableName == tblname).Select(a=>a.Field_ID).FirstOrDefault();
            var fieldinfo = Context_.Field_Tbl.Where(a=>a.Field_ID == id).FirstOrDefault();
            var fldmodel = new DB_DictionaryContext().Field_Tbl.ToList();
            using (Context_)
            {
                fieldinfo.Field_Description = Descript;
                fieldinfo.UpdatedDate = DateTime.Now;
                Context_.SaveChanges();
            }
            Context_ = new DB_DictionaryContext();
            return fldmodel;
        }
    }

    public class UpdateDatabaseModel
    {
        public DB_DictionaryContext Context_ { get; set; }

        public UpdateDatabaseModel()
        { }

        public IList<Database_Tbl> UpdateDatabase(string DBName, string server, string DBDescrip)
        {
            Context_ = new DB_DictionaryContext();
            var desc = Context_.Database_Tbl.Where(a => a.DB_Name == DBName && a.ServerName == server).FirstOrDefault();
            var dbmodel = new DB_DictionaryContext().Database_Tbl.ToList();
            var updateDate = DateTime.Now;
            using (Context_)
            {
                if (desc != null)
                {
                    desc.DB_Description = DBDescrip;
                    desc.UpdatedDate = updateDate;
                    Context_.SaveChanges();
                }
            }
            return dbmodel;
        }

    }
}