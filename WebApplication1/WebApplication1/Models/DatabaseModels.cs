using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;

namespace WebApplication1.Models
{
    public class dbModel
    {
        public string connectionstring = WebConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        public IList<Database_Tbl> dblist()
        {
            
            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                IList<Database_Tbl> _dbs = ImportDatabaseInformation(conn);
                ImportTablesInformation(conn,_dbs);
                var dbmodel = new DB_DictionaryContext().Database_Tbl.ToList();
                return dbmodel;
            }
            //return model;
        }

        public IList<Table_Tbl> tblList(IList<Database_Tbl> _dbs)
        {
            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                ImportTablesInformation(conn,_dbs);
                var tblModel = new DB_DictionaryContext().Table_Tbl.ToList();
                return tblModel;
            }
        }

        private IList<Database_Tbl> ImportDatabaseInformation(SqlConnection conn)
        {
            var dbname = "";
            var db_id = "";
            var filename = "";
            SqlCommand cmd = new SqlCommand("SELECT [dbid],[name],[filename] FROM [master].[dbo].sysdatabases WHERE dbid > 6", conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            IList<Database_Tbl> _dbs = new List<Database_Tbl>();
            adapter.Fill(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow Row = dt.Rows[i];
                db_id = Row["dbid"].ToString();
                dbname = Row["name"].ToString();
                filename = Row["filename"].ToString();
                _dbs.Add(new Database_Tbl { DB_ID = int.Parse(db_id), DB_Name = dbname, DB_Description = filename });
            }

            var EntityFdb = new DB_DictionaryContext();
            foreach (var record in _dbs)
            {
                EntityFdb.Database_Tbl.Add(record);
            }
            int dbId = EntityFdb.SaveChanges();

            return _dbs;
        }

        private void ImportTablesInformation(SqlConnection conn, IList<Database_Tbl> _dbs)
        {
            var tblname = "";
            var desc = "";
            //int dbId = 0;
            foreach(var dbrecord in _dbs)
            {
                SqlCommand cmd = new SqlCommand("SELECT [name],[type_desc] FROM [" +_dbs + "].[sys].tables", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                IList<Table_Tbl> _tbls = new List<Table_Tbl>();
                adapter.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow Row = dt.Rows[i];
                    tblname = Row["name"].ToString();
                    desc = Row["type_desc"].ToString();
                    _tbls.Add(new Table_Tbl { TBL_Name = tblname, TBL_Description = desc, DB_ID = 2});
                }

                var EntityFtbl = new DB_DictionaryContext();
                foreach (var tblrecord in _tbls)
                {
                    EntityFtbl.Table_Tbl.Add(tblrecord);
                }
                EntityFtbl.SaveChanges();
            }
        }

        private void ImportFields(SqlConnection conn, int tableId)
        {
            //do some import
        }
    }
}