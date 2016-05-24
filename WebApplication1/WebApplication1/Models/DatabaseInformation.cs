using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace WebApplication1.Models
{
    public class DatabaseMetaData : NetworkServers
    {
        public string id { get; set; }
        public string DatabaseName { get; set; }
        public string FileName { get; set; }
        public List<DatabaseMetaData> NewDatabaseList { get; set; }
        public List<string> DatabaseNamesList { get; set; }
    }

    public class ConnectionString : NetworkServers
    {
        //Member Fields
        public string connectionString_ { get; set; }
        private string AlternativeServerName { get; set; }

        //Default Constructor
        public ConnectionString()
        {

        }

        //Overloaded Constructor
        public string WindowsAuthentication(bool AlternativeServerNameClicked, string _ServerName, string _AlternativeServerName)
        {
            ServerName = _ServerName;
            AlternativeServerName = _AlternativeServerName;
            if (AlternativeServerNameClicked == true)
            {
                connectionString_ = WebConfigurationManager.AppSettings["DataSource"] + AlternativeServerName + WebConfigurationManager.AppSettings["Security"];
            }
            else
            {
                connectionString_ = WebConfigurationManager.AppSettings["DataSource"] + ServerName + WebConfigurationManager.AppSettings["Security"];
            }
            return connectionString_;
        }

        public string SQLAuthentication(string _ServerName, string _AlternativeServerName, string UserName, string Password, bool AlternativeServerNameClicked)
        {
            ServerName = _ServerName;
            AlternativeServerName = _AlternativeServerName;
            if (AlternativeServerNameClicked == true)
            {
                connectionString_ = WebConfigurationManager.AppSettings["DataSource"] + AlternativeServerName + WebConfigurationManager.AppSettings["InitialCatalog"] + UserName + WebConfigurationManager.AppSettings["Password"] + Password;

            }
            else
            {
                connectionString_ = WebConfigurationManager.AppSettings["DataSource"] + ServerName + WebConfigurationManager.AppSettings["InitialCatalog"] + UserName + WebConfigurationManager.AppSettings["Password"] + Password;
            }
            return connectionString_;
        }
    }

    public class ImportDatabaseMetadata : DatabaseMetaData
    {
        //Default Constructor
        public ImportDatabaseMetadata()
        {

        }

        //Overloaded Constructor
        public ImportDatabaseMetadata(string connectionString_)
        {
            using(SqlConnection connection_ = new SqlConnection(connectionString_))
            {
                SqlCommand Command = new SqlCommand("SELECT [dbid],[name],[filename],@@Servername as server FROM [master].[dbo].sysdatabases WHERE dbid > 6", connection_);
                SqlDataAdapter DataAdapter = new SqlDataAdapter(Command);
                DataTable DataSource = new DataTable();
                DataAdapter.Fill(DataSource);
                List<DatabaseMetaData> _DatabasesList = (   from a in DataSource.AsEnumerable()
                                                            select new DatabaseMetaData
                                                            {
                                                                id = a["dbid"].ToString(),DatabaseName = a["name"].ToString(),FileName = a["filename"].ToString(),ServerName = a["server"].ToString()
                                                            }).ToList();
                NewDatabaseList = new List<DatabaseMetaData>(_DatabasesList);
            }
            //GettingNames();   
        }

        public SelectList DatabasesList { get; set; } //Database Metadata
    }
}