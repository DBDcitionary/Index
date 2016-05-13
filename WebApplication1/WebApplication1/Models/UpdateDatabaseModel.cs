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
    //****************************
    //Update Database Description
    //****************************
    public class UpdateDatabaseModel
    {
        public DB_DictionaryContext Context_ { get; set; }//Setting Database Connection and Getting the Model.
        //Constructor with No Parameters.
        public UpdateDatabaseModel()
        { }
        //Updating Method for Database Description.
        public IList<Database_Tbl> UpdateDatabase(string DBName, string server, string Description)
        {
            Context_ = new DB_DictionaryContext();//Setting New Database Connection.
            var DatabaseRecord = Context_.Database_Tbl.Where(a => a.DB_Name == DBName && a.ServerName == server).FirstOrDefault();//Variable for Getting selected Database Information.
            var DatabaseModel_ = new DB_DictionaryContext().Database_Tbl.ToList();//Variable for Model with database Information.
            var UpdateDate = DateTime.Now; //Variable for Current Date Information
            using (Context_)
            {
                if (Description != null)//Checking if description has value.
                {
                    //Conditon to Check if the Record exist or Not.
                    if (DatabaseRecord != null)
                    {
                        DatabaseRecord.DB_Description = Description;
                        DatabaseRecord.UpdatedDate = UpdateDate;
                        Context_.SaveChanges();
                    }
                }
            }
            return DatabaseModel_;
        }

    }
}