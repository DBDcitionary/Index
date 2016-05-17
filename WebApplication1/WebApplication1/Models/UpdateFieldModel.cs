using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class UpdateFieldModel : UpdateDatabaseModel
    {
        //Constructor With No Parameters
        public UpdateFieldModel()
        {
            NewDescription = "";
            NewtblName = "";
            NewfldName = "";
        }

        public IList<Field_Tbl> UpdateField(string Description, string DBName, string tblname, string fldName)
        { 
            Context_ = new DB_DictionaryContext();
            var id = Context_.Field_Tbl.Where(a => a.DBName == DBName && a.TableName == tblname && a.Field_Name == fldName).Select(a => a.Field_ID).FirstOrDefault();
            var fieldinfo = Context_.Field_Tbl.Where(a => a.Field_ID == id).FirstOrDefault();
            var fldmodel = new DB_DictionaryContext().Field_Tbl.ToList();
            using (Context_)
            {
                if(Description != null)
                {
                    if (fieldinfo != null)
                    {
                        fieldinfo.Field_Description = Description;
                        fieldinfo.UpdatedDate = DateTime.Now;
                        Context_.SaveChanges();
                    }
                }
               
            }
            Context_ = new DB_DictionaryContext();
            return fldmodel;
        }

        //Accessor
        public string getTableName()
        {
            return NewtblName;
        }

        private string getFieldDescription()
        {
            return NewDescription;
        }

        public string getFieldName()
        {
            return NewfldName;
        }

        //Member Variables
        private string NewDescription;
        private  string NewtblName;
        private string NewfldName;
    }
}