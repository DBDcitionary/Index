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
    public class UpdateDatabaseModel
    {
        public DB_DictionaryContext Context_ { get; set; }

        //Member Variables
        public string NewDBName;
        private string Newserver;
        private string NewDescription;

        //Constructor with No Parameters.
        public UpdateDatabaseModel()
        {
            NewDBName = "";
            Newserver = "";
            NewDescription = "";
        }

        //Updating Method for Database Description.
        public IList<Database_Tbl> UpdateDatabase(string DatabseName, string Server, string DatabaseDescription)
        {
            Context_ = new DB_DictionaryContext();//Setting New Database Connection.
            var DatabaseRecord = Context_.Database_Tbl.Where(a => a.DB_Name == DatabseName && a.ServerName == Server).FirstOrDefault();
            var DatabaseModel_ = new DB_DictionaryContext().Database_Tbl.ToList();//Variable for Model with database Information.
            var UpdateDate = DateTime.Now; //Variable for Current Date Information
            using (Context_)
            {
                if (DatabaseDescription != null)//Checking if description has value.
                {
                    //Conditon to Check if the Record exist or Not.
                    if (DatabaseRecord != null)
                    {
                        DatabaseRecord.DB_Description = DatabaseDescription;
                        DatabaseRecord.UpdatedDate = UpdateDate;
                        Context_.SaveChanges();
                    }
                }
            }
            return DatabaseModel_;
        }

        //Accessors
        public string getDBName()
        {
            return NewDBName;
        }

        private string getServer()
        {
            return NewDescription;
        }

        private string getDescription()
        {
            return NewDescription;
        }

        public void setDBName(string DatabaseName)
        {
            NewDBName = DatabaseName;
        }

        public void setSever(string ServerName)
        {
            Newserver = ServerName;
        }

        public void setDatabaseDEsciption( string Description)
        {
            NewDescription = Description;
        }

       

    }
}