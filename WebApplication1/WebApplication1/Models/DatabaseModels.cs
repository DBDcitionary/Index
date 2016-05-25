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
    public class ImportingModel 
    {
        //Default Constructor
        public ImportingModel()
        {

        }

        //Overloaded Constructor
        public ImportingModel(string connection, string Database)
        {
            ImportingDatabase import = new ImportingDatabase();
            import.database(Database, connectionstring = connection);
        }

        //Member Variables
        public DB_DictionaryContext Context_ { get; set; }
        public string connectionstring;
        public Database_Tbl DatabaseMetadata { get; set; }
    }

    class ImportingDatabase : ImportingModel
    {
        //Default Constructor
        public ImportingDatabase()
        {

        }

        //Method to Import Database Information
        public Database_Tbl database(string Database, string connectionstring)
        {
            ImportDatabaseMetadata database_ = new ImportDatabaseMetadata(connectionstring);
            DatabaseMetadata = new Database_Tbl();
            Context_ = new DB_DictionaryContext();
            foreach (var db in database_.NewDatabaseList.Where(a => a.DatabaseName == Database))
            {
                getserverName(db);
                var exist = Context_.Database_Tbl.Where(a => a.ServerName == db.ServerName && a.DB_Name == db.DatabaseName && a.FK_Databaseid == db.id).FirstOrDefault();
                if (exist != null)
                {
                    UpdateRecord(exist, db);
                }
                else
                {
                    DatabaseMetadata.DB_Name = databaseName;
                    DatabaseMetadata.DB_Description = description;
                    DatabaseMetadata.ServerName = serverName;
                    DatabaseMetadata.CreatedDate = DateTime.Now;
                    DatabaseMetadata.FK_Databaseid = db.id;
                    CreateNewRecord(DatabaseMetadata);
                }
                ImportingTables Tables = new ImportingTables();
                Tables.table(connectionstring, db);
            }
            return DatabaseMetadata;
        }

        //Accessor
        public string getserverName(DatabaseMetaData database)
        {
            serverName = database.ServerName;
            getdatabaseName(database);
            return serverName;
        }

        public string getdatabaseName(DatabaseMetaData database)
        {
            databaseName = database.DatabaseName;
            getdescription(database);
            return databaseName;
        }

        public string getdescription(DatabaseMetaData database)
        {
            description = database.FileName;
            return description;
        }

        //Method to Create New record
        public void CreateNewRecord(Database_Tbl db_)
        {
            using (Context_)
            {
                Context_.Database_Tbl.Add(db_);
                Context_.SaveChanges();
            }
            Context_ = new DB_DictionaryContext();
        }

        //Method to Update if record exist
        public void UpdateRecord(Database_Tbl existingRecord, DatabaseMetaData db_)
        {
            using (Context_)
            {
                existingRecord.DB_Name = db_.DatabaseName;
                existingRecord.DB_Description = db_.FileName;
                existingRecord.UpdatedDate = DateTime.Now;
                Context_.SaveChanges();
            }
            Context_ = new DB_DictionaryContext();
        }

        //Member Variables
        public string serverName;
        public string databaseName;
        private string description;
    }

    class ImportingTables : ImportingDatabase
    {
        //Default Condtructor
        public ImportingTables()
        {

        }

        //Method to Import
        public Table_Tbl table(string conn_, DatabaseMetaData databaseInfor_)
        {
            Table_Tbl tables = new Table_Tbl();
            Context_ = new DB_DictionaryContext();
            using (SqlConnection conn = new SqlConnection(conn_))
            {
                SqlCommand Command = new SqlCommand("SELECT object_id,name FROM ["+databaseInfor_.DatabaseName +"].[sys].tables", conn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(Command);
                DataTable dataSource = new DataTable();
                dataAdapter.Fill(dataSource);
                id(databaseInfor_);
                foreach (var db in dataSource.AsEnumerable())
                {
                    gettableName(db);
                    var exist = Context_.Table_Tbl.Where(a => a.DB_ID == DatabaseID && a.TBL_Name == tableName && a.FK_Tableid == FK_Tableid).FirstOrDefault();
                    if (exist != null)
                    {
                        UpdateRecord(exist, db);
                    }
                    else
                    { 
                        tables.TBL_Name = tableName;
                        tables.TBL_Description = tableDescription;
                        tables.FK_Tableid = FK_Tableid;
                        tables.CreatedDate = DateTime.Now;
                        tables.DB_ID = DatabaseID;
                        CreateNewRecord(tables);
                    }
                    ImportingColumns col = new ImportingColumns();
                    col.ImportFields(conn_, db, databaseInfor_);
                }
            }
            return tables;
        }

        //Method to Create New Record
        private void CreateNewRecord(Table_Tbl tblrecord)
        {
            using (Context_)
            {
                Context_.Table_Tbl.Add(tblrecord);
                Context_.SaveChanges();
            }
            Context_ = new DB_DictionaryContext();
        }

        //Method to Update record if Record exist
        private void UpdateRecord(Table_Tbl existingRecord, DataRow tblrecord)
        {
            using (Context_)
            {
                existingRecord.FK_Tableid = FK_Tableid;
                existingRecord.TBL_Name = tableName;
                existingRecord.UpdatedDate = DateTime.Now;
                Context_.SaveChanges();
            }
            Context_ = new DB_DictionaryContext();
        }

        //Accessors
        private int id(DatabaseMetaData databaseInfor_)
        {
            List<int> id = Context_.Database_Tbl.Where(a => a.DB_Name == databaseInfor_.DatabaseName && a.FK_Databaseid == databaseInfor_.id && a.ServerName == databaseInfor_.ServerName).
                                                    Select(a => a.DB_ID).ToList();
            return DatabaseID = id[0];
        }

        private string gettableName(DataRow databaseInfor_)
        {
            tableName = databaseInfor_["name"].ToString();
            gettableDescription(databaseInfor_);
            return tableName;
        }

        private string gettableDescription(DataRow databaseInfor_)
        {
            tableDescription = "";
            getFK_Tableid(databaseInfor_);
            return tableDescription;
        }

        private string getFK_Tableid(DataRow databaseInfor_)
        {
            FK_Tableid = databaseInfor_["object_id"].ToString();
            return FK_Tableid;
        }

        //Member Variable
        private int DatabaseID;
        public string tableName;
        private string tableDescription;
        private string FK_Tableid;
    }

    class ImportingColumns : ImportingTables
    {
        //Default Constructor
        public ImportingColumns()
        {

        }

        //Method for Importing
        public void ImportFields(string connection, DataRow _tbls, DatabaseMetaData databaseInfor_)
        {
            getdataDatabase(databaseInfor_, _tbls);
            Field_Tbl columns = new Field_Tbl();
            Context_ = new DB_DictionaryContext();
            using (SqlConnection conn = new SqlConnection(connection))
            {
                SqlCommand command = new SqlCommand(@"SELECT    sc.column_id as colid,
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
                                                    FROM	" + databaseName + @".information_schema.tables tbl 
                                                    INNER JOIN " + databaseName + @".information_schema.columns col 
                                                        ON col.table_name = tbl.table_name 
                                                    INNER JOIN " + databaseName + @".sys.columns sc 
                                                        ON col.COLUMN_NAME = sc.name
                                                    INNER JOIN " + databaseName + @".sys.objects o
                                                        ON o.object_id = sc.object_id
                                                    LEFT JOIN (SELECT	colProp.major_id,
					                                                    colProp.minor_id,
					                                                    colProp.value as ColumnDescription 
			                                                    FROM " + databaseName + @".sys.extended_properties colProp) colDesc 
                                                        ON colDesc.major_id = sc.object_id 
			                                                    AND colDesc.minor_id = sc.column_id
                                                    WHERE tbl.table_name = '" + tableName + "' and o.name = '" + tableName + "'", conn);
                SqlDataAdapter DataAdapter = new SqlDataAdapter(command);
                DataTable DataSource = new DataTable();
                DataAdapter.Fill(DataSource);
                foreach(var item in DataSource.AsEnumerable())
                {
                    getfieldName(item);
                    var exist = Context_.Field_Tbl.Where(a => a.TBL_ID ==  tableID && a.Field_Name == fieldName && a.FK_Fieldid == FK_Fieldid).FirstOrDefault();
                    if (exist != null)
                    {
                        UpdateFeildRecord(exist, item);
                    }
                    else
                    { 
                        columns.ServerName = serverName;
                        columns.DBName = databaseName;
                        columns.TableName = tableName;
                        columns.SchemaName = schemaName;
                        columns.Field_Name = fieldName;
                        columns.DataType = dataType;
                        columns.Prec = precision;
                        columns.ColLength = Length;
                        columns.is_null = nullable;
                        columns.Field_Description = fieldDescription;
                        columns.TBL_ID = tableID;
                        columns.CreatedDate = DateTime.Now;
                        columns.FK_Fieldid = FK_Fieldid;
                        CreateNewFieldRecord(columns);
                    } 
                }
            }
        }

        //Method to create new record
        public void CreateNewFieldRecord(Field_Tbl record)
        {
            using (Context_)
            {
                Context_.Field_Tbl.Add(record);
                Context_.SaveChanges();
            }
            Context_ = new DB_DictionaryContext();
        }

        //Method to Update existing record
        public void UpdateFeildRecord(Field_Tbl fieldinfo, DataRow record)
        {
            using (Context_)
            {
                fieldinfo.ServerName = serverName;
                fieldinfo.DBName = databaseName;
                fieldinfo.TableName = tableName;
                fieldinfo.SchemaName = schemaName;
                fieldinfo.Field_Name = fieldName;
                fieldinfo.DataType = dataType;
                fieldinfo.Prec = precision;
                fieldinfo.ColLength = Length;
                fieldinfo.is_null = nullable;
                fieldinfo.Field_Description = fieldDescription;
                fieldinfo.TBL_ID = tableID;
                fieldinfo.UpdatedDate = DateTime.Now;
                fieldinfo.FK_Fieldid = FK_Fieldid;
                Context_.SaveChanges();
            }
            Context_ = new DB_DictionaryContext();
        }

        //Accessors
        public string getdataDatabase(DatabaseMetaData databaseInfor_, DataRow tableInfor)
        {
            databaseName = databaseInfor_.DatabaseName;
            gettableName(tableInfor);
            getserver(databaseInfor_);
            return databaseName;
        }

        public string getserver(DatabaseMetaData databaseInfor_)
        {
            serverName = databaseInfor_.ServerName;
            return serverName;
        }

        public string gettableName(DataRow tableInfor)
        {
            tableName = tableInfor["name"].ToString();
            gettableId(tableInfor);
            return tableName;
        }

        public int gettableId(DataRow tableInfor)
        {
            var fk = tableInfor["Object_id"].ToString();
            List<int> id = new List<int>();
            using (Context_ = new DB_DictionaryContext())
            {
                id = Context_.Table_Tbl.Where(a => a.TBL_Name == tableName && a.FK_Tableid == fk).
                                                                       Select(a => a.TBL_ID).ToList();
            }  
            return tableID = id[0];
        }

        public string getfieldName(DataRow fieldInfor)
        {
            fieldName = fieldInfor["ColumnName"].ToString();
            getschemaName(fieldInfor);
            return fieldName;
        }

        public string getschemaName(DataRow fieldInfor)
        {
            schemaName = fieldInfor["SchemaName"].ToString();
            getdataType(fieldInfor);
            return schemaName;
        }

        public string getdataType(DataRow fieldInfor)
        {
            dataType = fieldInfor["ColumnDataType"].ToString();
            getprecision(fieldInfor);
            return dataType;
        }

        public int getprecision(DataRow fieldInfor)
        {
            if(fieldInfor["Prec"].ToString().Count() == 0)
            {
                precision = 0;
            }
            else
            {
                precision = int.Parse(fieldInfor["Prec"].ToString());
            }  
            getLength(fieldInfor);
            return precision;
        }

        public int getLength(DataRow fieldInfor)
        {
            if(fieldInfor["LEN"].ToString().Count() == 0)
            {
                Length = 0;
            }
            else
            {
                Length = int.Parse(fieldInfor["LEN"].ToString());
            }
            getnullable(fieldInfor);
            return Length;
        }

        public string getnullable(DataRow fieldInfor)
        {
            nullable = fieldInfor["IS_NULLABLE"].ToString();
            getFK_Fieldid(fieldInfor);
            return nullable;
        }

        public string getFK_Fieldid(DataRow fieldInfor)
        {
            FK_Fieldid = fieldInfor["colid"].ToString();
            return FK_Fieldid;
        }

        //Member Variables
        private string schemaName;
        private string fieldName;
        private string fieldDescription = "";
        private string dataType;
        private int precision;
        private int Length;
        private string nullable;
        private int tableID;
        private string FK_Fieldid;
    }
}