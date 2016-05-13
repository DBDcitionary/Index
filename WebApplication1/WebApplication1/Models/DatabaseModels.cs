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
        string connectionstring;
        public DB_DictionaryContext Context_ { get;  set; }

        public dbModel(string conn)
        {
            connectionstring = conn;
            Context_ = new DB_DictionaryContext();
        }

        public dbModel()
        {

        }

        public List<string> database(string connectionstring)
        {
            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                SqlCommand cmd = new SqlCommand("SELECT [dbid],[name],[filename],@@Servername as server FROM [master].[dbo].sysdatabases WHERE dbid > 6", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                List<string> db = new List<string>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow Rows = dt.Rows[i];
                    var databname = Rows["name"].ToString();
                    db.Add(databname);
                }
                return db;
            }
        }

        public IList<Database_Tbl> dblist(string database)
        {
            var database_ = database;
            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                conn.Open();
                IList<Database_Tbl> _dbs = ImportDatabaseInformation(conn,database_);
                ImportTablesInformation(conn, _dbs);
                var dbmodel = new DB_DictionaryContext().Database_Tbl.ToList();
                return dbmodel;
            }
        }

        private IList<Database_Tbl> HttpNotFound()
        {
            throw new NotImplementedException();
        }

        private IList<Database_Tbl> ImportDatabaseInformation(SqlConnection conn, string database)
        {
            var dbname = "";
            var filename = "";
            var server = "";
            var CreatedDate = DateTime.Now;
            var databaseid = "";
            SqlCommand cmd = new SqlCommand("SELECT [dbid],[name],[filename],@@Servername as server FROM [master].[dbo].sysdatabases WHERE dbid > 6 AND name = '"+database+"'", conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            IList<Database_Tbl> _dbs = new List<Database_Tbl>();
            adapter.Fill(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow Row = dt.Rows[i];
                dbname = Row["name"].ToString();
                filename = Row["filename"].ToString();
                server = Row["server"].ToString();
                databaseid = Row["dbid"].ToString();
                _dbs.Add(new Database_Tbl { DB_Name = dbname, DB_Description = filename, ServerName = server, CreatedDate = CreatedDate, FK_Databaseid =  databaseid});
            }

            foreach(var db_ in _dbs)
            {
                   var databaseinfo= Context_.Database_Tbl.Where(a => a.ServerName == db_.ServerName && a.DB_Name == db_.DB_Name).FirstOrDefault();
                   if (databaseinfo == null)
                   {
                        CreateNewRecord(db_);
                   }
                   else
                   {
                        UpdateRecord(databaseinfo, db_);
                   }
            }
            return _dbs;
        }

        private void UpdateRecord(Database_Tbl existingRecod, Database_Tbl db_)
        {
            using (Context_)
            {
                existingRecod.DB_Name = db_.DB_Name;
                existingRecod.DB_Description = db_.DB_Description;
                Context_.SaveChanges();
            }

            Context_ = new DB_DictionaryContext();
        }

        private void CreateNewRecord(Database_Tbl db_)
        {
            using (Context_)
            {
                Context_.Database_Tbl.Add(db_);
                Context_.SaveChanges();
            }
            Context_ = new DB_DictionaryContext();
        }

        private void ImportTablesInformation(SqlConnection conn, IList<Database_Tbl> _dbs)
        {
            var tblname = "";
            var desc = "";
            var CreatedDate = DateTime.Now;
            var tableid = "";
            IList<Table_Tbl> _tbls = new List<Table_Tbl>();
            var dbnames = _dbs.Select(a => a);
            //var dbnames = Context_.Database_Tbl.Select(a => a);
            foreach (var dbrecord in dbnames)
            {
                string DBName = dbrecord.DB_Name;
                var ForeignKeyDB = dbrecord.FK_Databaseid;
                //var DBid = dbrecord.DB_ID;

                SqlCommand cmd = new SqlCommand("SELECT object_id,name FROM ["+DBName+"].[sys].tables", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                var id = Context_.Database_Tbl.Where(a=>a.DB_Name == DBName && a.FK_Databaseid == ForeignKeyDB).Select(a => a.DB_ID).Single().ToString();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow Row = dt.Rows[i];
                    tableid = Row["object_id"].ToString();
                    tblname = Row["name"].ToString();
                    _tbls.Add(new Table_Tbl { TBL_Name = tblname, TBL_Description = desc, DB_ID = int.Parse(id), CreatedDate = CreatedDate, FK_Tableid = tableid});
                }

                foreach (var tblrecord in _tbls)
                {
                    var tableinfo = Context_.Table_Tbl.Where(a => a.FK_Tableid == tblrecord.FK_Tableid && a.TBL_Name == tblrecord.TBL_Name &&a.DB_ID == tblrecord.DB_ID).FirstOrDefault();
                    if(tableinfo == null)
                    {
                        CreateNewTableRecord(tblrecord);
                    } 
                    else
                    {
                        UpdateTableRecord(tableinfo, tblrecord);
                    }
                }
            }
            ImportFields(conn, _tbls);
        }

        private void CreateNewTableRecord(Table_Tbl tblrecord)
        {
            using (Context_)
            {
                Context_.Table_Tbl.Add(tblrecord);
                Context_.SaveChanges();
            }
                Context_ = new DB_DictionaryContext();
        }

        private void UpdateTableRecord(Table_Tbl tableinfo, Table_Tbl tblrecord)
        {
            using (Context_)
            {
                tableinfo.FK_Tableid = tblrecord.FK_Tableid;
                tableinfo.TBL_Name = tblrecord.TBL_Name;
                tableinfo.TBL_Description = tblrecord.TBL_Description;
                tableinfo.UpdatedDate = DateTime.Now;
                Context_.SaveChanges();
            }
                Context_ = new DB_DictionaryContext();
        }

        private void ImportFields(SqlConnection conn, IList<Table_Tbl> _tbls)
        {
            DB_DictionaryContext db = new DB_DictionaryContext();
            var server = "";
            var DBName = "";
            var TableName = "";
            var SchemaName = "";
            var ColName = "";
            var dataType = "";
            int ? precision = 0;
            int ? length = 0;
            var is_null = "";
            var description = "";
            var CreatedDate = DateTime.Now;
            var fieldid = ""; 

            var join = from table in _tbls
                       join database in db.Database_Tbl on table.DB_ID equals database.DB_ID
                       select new { table.TBL_Name, database.DB_Name,database.ServerName,database.DB_ID,table.TBL_ID,table.FK_Tableid };
            var tblnames = join.Select(a => new{ a.TBL_Name,a.FK_Tableid,a.DB_ID }).ToList();

            foreach (var tblrecord in tblnames)
            {
                string ForeignKeyTBL = tblrecord.FK_Tableid;
                string tName = tblrecord.TBL_Name;
                int Dataid = tblrecord.DB_ID;

                var datab = join.Where(a => a.TBL_Name == tName).Select(a => a.DB_Name).ToArray()[0];
                SqlCommand cmd = new SqlCommand(@"SELECT    sc.column_id as colid,
		                                                    @@Servername as Server ,
		                                                    tbl.TABLE_CATALOG as DBName ,
		                                                    tbl.table_name as TableName,
		                                                    tbl.table_schema  as SchemaName,
		                                                    col.column_name as ColumnName,
		                                                    col.data_type as ColumnDataType,
		                                                    sc.precision as Prec,
		                                                    Character_Maximum_Length as LEN,
		                                                    col.IS_NULLABLE,
		                                                    ISNULL( colDesc.ColumnDescription,'') as ColumnDescription 
                                                    FROM	"+datab+@".information_schema.tables tbl 
                                                    INNER JOIN "+datab+@".information_schema.columns col 
                                                        ON col.table_name = tbl.table_name 
                                                    INNER JOIN "+datab+@".sys.columns sc 
                                                        ON col.COLUMN_NAME = sc.name
                                                    INNER JOIN "+datab+@".sys.objects o
                                                        ON o.object_id = sc.object_id
                                                    LEFT JOIN (SELECT	colProp.major_id,
					                                                    colProp.minor_id,
					                                                    colProp.value as ColumnDescription 
			                                                    FROM "+datab+@".sys.extended_properties colProp) colDesc 
                                                        ON colDesc.major_id = sc.object_id 
			                                                    AND colDesc.minor_id = sc.column_id
                                                    WHERE tbl.table_name = '"+ tName + "' and o.name = '"+ tName + "'", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                IList<Field_Tbl> _fld = new List<Field_Tbl>();
                adapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow Row = dt.Rows[i];
                    server = Row["Server"].ToString();
                    DBName = Row["DBName"].ToString();
                    TableName = Row["TableName"].ToString();
                    SchemaName = Row["SchemaName"].ToString();
                    ColName = Row["ColumnName"].ToString();
                    dataType = Row["ColumnDataType"].ToString();
                    fieldid = Row["colid"].ToString();

                    if (Row["Prec"].ToString().Count() == 0)
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
                    var id = Context_.Table_Tbl.Where(a => a.TBL_Name == tName && a.DB_ID == Dataid && a.FK_Tableid == ForeignKeyTBL).Select(a => a.TBL_ID).SingleOrDefault().ToString();
                    _fld.Add(new Field_Tbl { ServerName = server, DBName = DBName, TableName = TableName, SchemaName = SchemaName, Field_Name = ColName, DataType = dataType, Prec = precision, ColLength = length, is_null = is_null, Field_Description = description, TBL_ID = int.Parse(id), CreatedDate = CreatedDate, FK_Fieldid = fieldid });
                    foreach (var record in _fld)
                    {
                        var fieldinfo = Context_.Field_Tbl.Where(a => a.FK_Fieldid == record.FK_Fieldid && a.TBL_ID == record.TBL_ID).FirstOrDefault();
                        if (fieldinfo == null)
                        {
                            CreateNewFieldRecord(record);
                        }
                        else
                        {
                            UpdateFeildRecord(fieldinfo, record);
                        }
                    }
                }
            }
        }

        private void CreateNewFieldRecord(Field_Tbl record)
        {
            using (Context_)
            {
                Context_.Field_Tbl.Add(record);
                Context_.SaveChanges();
            }
            Context_ = new DB_DictionaryContext();
        }

        private void UpdateFeildRecord(Field_Tbl fieldinfo, Field_Tbl record)
        {
            using (Context_)
            {
                fieldinfo.ServerName = record.ServerName;
                fieldinfo.DBName = record.DBName;
                fieldinfo.TableName = record.TableName;
                fieldinfo.SchemaName = record.SchemaName;
                fieldinfo.Field_Name = record.Field_Name;
                fieldinfo.DataType = record.DataType;
                fieldinfo.Prec = record.Prec;
                fieldinfo.ColLength = record.ColLength;
                fieldinfo.is_null = record.is_null;
                fieldinfo.Field_Description = record.Field_Description;
                fieldinfo.TBL_ID = record.TBL_ID;
                fieldinfo.UpdatedDate = record.UpdatedDate;
                fieldinfo.FK_Fieldid = record.FK_Fieldid;
            }
            Context_ = new DB_DictionaryContext();
        }
    }
}