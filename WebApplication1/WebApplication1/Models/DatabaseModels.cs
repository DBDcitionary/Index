using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;
using System.Configuration;

namespace WebApplication1.Models
{
    public class dbModel
    {
        //ConnectionStringSettings conset = new ConnectionStringSettings("conn", WebConfigurationManager.ConnectionStrings[2].ConnectionString);
        string connectionstring;// = WebConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        public dbModel(string conn)
        {
            connectionstring = conn;
        }
        public dbModel()
        { }

        public IList<Database_Tbl> dblist()
        {
            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                conn.Open();
                IList<Database_Tbl> _dbs = ImportDatabaseInformation(conn);
                ImportTablesInformation(conn, _dbs);
                var dbmodel = new DB_DictionaryContext().Database_Tbl.ToList();
                return dbmodel;
            }
        }

        private IList<Database_Tbl> HttpNotFound()
        {
            throw new NotImplementedException();
        }

        private IList<Database_Tbl> ImportDatabaseInformation(SqlConnection conn)
        {
            var dbname = "";
            var db_id = "";
            var filename = "";
            var server = "";
            var CreatedDate = DateTime.Now;
            SqlCommand cmd = new SqlCommand("SELECT [dbid],[name],[filename],@@Servername as server FROM [master].[dbo].sysdatabases WHERE dbid > 5", conn);
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
                server = Row["server"].ToString();
                _dbs.Add(new Database_Tbl { DB_ID = int.Parse(db_id), DB_Name = dbname, DB_Description = filename, ServerName = server, CreatedDate = CreatedDate });
            }

            var EntityFdb = new DB_DictionaryContext();
            foreach (var record in _dbs)
            {
                //var recordExist = new DB_DictionaryContext().Database_Tbl.Select(a => new { a.DB_ID, a.DB_Name, a.ServerName}).Where(a => a.DB_Name == dbname && a.ServerName == server).Single();
                //if (recordExist != null)
                //{
                //    EntityFdb.Entry(db_id).CurrentValues.SetValues(record);
                //    EntityFdb.SaveChanges();
                //}
                //else
                    EntityFdb.Database_Tbl.Add(record);
            }
            int dbId = EntityFdb.SaveChanges();

            return _dbs;
        }

        private void ImportTablesInformation(SqlConnection conn, IList<Database_Tbl> _dbs)
        {
            var tblname = "";
            var desc = "";
            var CreatedDate = DateTime.Now;
            IList<Table_Tbl> _tbls = new List<Table_Tbl>();
            var dbnames = _dbs.Select(a => a.DB_Name);
            foreach (var dbrecord in dbnames)
            {
                SqlCommand cmd = new SqlCommand("SELECT [name],[type_desc] FROM [" + dbrecord + "].[sys].tables", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                var dbid = _dbs.Where(a => a.DB_Name == dbrecord).Select(a => new { a.DB_ID }.DB_ID).Single().ToString();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow Row = dt.Rows[i];
                    tblname = Row["name"].ToString();
                    desc = Row["type_desc"].ToString();
                    _tbls.Add(new Table_Tbl { TBL_Name = tblname, TBL_Description = desc, DB_ID = int.Parse(dbid), CreatedDate = CreatedDate });
                }

                var EntityFtbl = new DB_DictionaryContext();
                foreach (var tblrecord in _tbls)
                {
                    EntityFtbl.Table_Tbl.Add(tblrecord);
                }
                EntityFtbl.SaveChanges();
            }
            ImportFields(conn, _tbls);
        }

        private void ImportFields(SqlConnection conn, IList<Table_Tbl> _tbls)
        {
            DB_DictionaryContext db = new DB_DictionaryContext();
            var  server = "";
            var DBName = "";
            var TableName = "";
            var SchemaName = "";
            var ColName = "";
            var dataType = "";
            int ? precision = 0;
            int ? length = 0;
            var is_null = "";
            var description = "";
            DateTime CreatedDate = new DateTime();
            CreatedDate = DateTime.Now;
            CreatedDate.ToLongDateString();
            var join = from table in _tbls
                       join database in db.Database_Tbl on table.DB_ID equals database.DB_ID
                       select new { table.TBL_Name, database.DB_Name,database.ServerName,table.TBL_ID };
            var tblnames = join.Select(a => a.TBL_Name).ToList();
            foreach (var tblrecord in tblnames)
            {
                var datab = join.Where(a => a.TBL_Name == tblrecord.ToString()).Select(a => a.DB_Name).ToArray()[0];
                SqlCommand cmd = new SqlCommand("SELECT @@Servername as Server ,tbl.TABLE_CATALOG as DBName ,tbl.table_name as TableName,tbl.table_schema  as SchemaName,col.column_name as ColumnName,col.data_type as ColumnDataType,Numeric_Precision as Prec ,Character_Maximum_Length as LEN ,col.IS_NULLABLE,case when colDesc.ColumnDescription IS  Null then ' ' else Convert(varchar(255), colDesc.ColumnDescription) end  as ColumnDescription FROM " + datab.ToString() + ".information_schema.tables tbl INNER JOIN "+ datab.ToString() + ".information_schema.columns col ON col.table_name = tbl.table_name LEFT JOIN "+ datab.ToString() + ".sys.extended_properties tableProp ON tableProp.major_id = object_id(tbl.table_schema+'.'+tbl.table_name) AND tableProp.minor_id = 0 AND tableProp.name = 'MS_Description' LEFT JOIN (SELECT sc.object_id, sc.column_id, sc.name, colProp.[value] AS ColumnDescription FROM "+ datab.ToString() + ".sys.columns sc INNER JOIN "+ datab.ToString() + ".sys.extended_properties colProp ON colProp.major_id = sc.object_id AND colProp.minor_id = sc.column_id AND colProp.name = 'MS_Description') colDesc ON colDesc.object_id = object_id(tbl.table_schema+'.'+tbl.table_name) AND colDesc.name = col.COLUMN_NAME WHERE tbl.table_name = '" + tblrecord+"'", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                IList<Field_Tbl> _fld = new List<Field_Tbl>();
                adapter.Fill(dt);
                for(int i= 0; i < dt.Rows.Count; i++)
                {
                    DataRow Row = dt.Rows[i];
                    server = Row["Server"].ToString();
                    DBName = Row["DBName"].ToString();
                    TableName = Row["TableName"].ToString();
                    SchemaName = Row["SchemaName"].ToString();
                    ColName = Row["ColumnName"].ToString();
                    dataType = Row["ColumnDataType"].ToString();
                    if(Row["Prec"].ToString().Count() == 0)
                    {
                        precision = 0;
                    }
                    else
                    {
                        precision = int.Parse(Row["Prec"].ToString());
                    }

                    if (Row["LEN"].ToString().Count() == 0)
                    {
                        length = 0;
                    }
                    else
                    {
                        length = int.Parse(Row["LEN"].ToString());
                    }
                    is_null = Row["Is_Nullable"].ToString();
                    description = Row["ColumnDescription"].ToString();
                    var tblbid = join.Where(a=>a.TBL_Name== tblrecord && a.ServerName == server).Select(a => a.TBL_ID).ToArray()[0];
                    _fld.Add(new Field_Tbl { ServerName = server, DBName = DBName, TableName = TableName, SchemaName = SchemaName, Field_Name = ColName, DataType = dataType, Prec = precision, ColLength = length, is_null = is_null,Field_Description = description, TBL_ID = int.Parse(tblbid.ToString()),CreatedDate = CreatedDate});
                }

                var EntityFld = new DB_DictionaryContext();
                foreach (var record in _fld)
                {
                    EntityFld.Field_Tbl.Add(record);
                }
                EntityFld.SaveChanges();
                conn.Close();
            }
        }
    }
}