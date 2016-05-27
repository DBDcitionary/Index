using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Models
{
    public class ListingDatabaseInforModels : Database_Tbl
    {
        //Default Constructor
        public ListingDatabaseInforModels()
        {

        }

        //Overloaded Constructor
        public ListingDatabaseInforModels(List<Database_Tbl> Model)
        {
            NewModel = Model;
            getServers(Model);
            getDatabases(Model, ServerName);
            getTables(Model);
        }

        public void SubmitForm(List<Database_Tbl> Model)
        {
            getServers(Model);
            getDatabases(Model, ServerName);
            getTables(Model);
        }

        //Accessors
        private IEnumerable<Database_Tbl> getServers(List<Database_Tbl> Model)
        {
            return Servers = Model;
        }

        private IEnumerable<Database_Tbl> getDatabases(List<Database_Tbl> Model, string _serverName)
        {
            getdatabaseDescription(Model);
            if (_serverName != null)
            {
                return Databases = Model.Where(a => a.ServerName == _serverName);
            }
            else
                return Databases = new List<Database_Tbl>();
        }

        private IEnumerable<Table_Tbl> getTables(List<Database_Tbl> Model)
        {
            getTablesDescription(Model);
            if (DB_Name != " ")
            {
                var tableInformation = Model.Where(a => a.DB_Name == DB_Name).Select(b => b.Table_Tbl);
                var tableNames = new List<Table_Tbl>();
                foreach (var item in tableInformation)
                {
                    tableNames.AddRange(item);
                    IList<int> FieldCount = item.Where(a => a.TBL_Name == TBL_Name).Select(a => a.Field_Tbl.Count).ToList();
                    test(FieldCount);
                    if(FieldCount.Count != 0)
                    {
                        getTableCounts(FieldCount[0].ToString());
                    }
                }
                return Tables = tableNames;
            }
            else
                return Tables = new List<Table_Tbl>();
        }

        private string getdatabaseDescription(List<Database_Tbl> Model)
        {
            var description = Model.Where(a => a.DB_Name == DB_Name).Select(a => a.DB_Description);
            var newDescription = "";
            foreach(var item in description)
            {
                newDescription = item.ToString();
            }
            return DB_Description = newDescription;
        }
        
        private string getTablesDescription(List<Database_Tbl> Model)
        {
            var description = Model.Where(a => a.DB_Name == DB_Name).Select(b => b.Table_Tbl);
            var newDescription = new List<string>();
            foreach (var item in description)
            {
                newDescription = item.Where(a=>a.TBL_Name == TBL_Name).Select(a=>a.TBL_Description).ToList();
            }

            if(newDescription.Count != 0)
            {
                return TBL_Description = newDescription[0].ToString();
            }
            else
            return TBL_Description = "";
        }

        private string getTableCounts(string Count)
        { 
            return FieldCounts = Count;
        }

        public IList<int> test(IList<int> test1)
        { 
            return Fields = test1;
        }

        //Member Variables
        public IEnumerable<Database_Tbl> Servers;
        public IEnumerable<Database_Tbl> Databases;
        public IEnumerable<Table_Tbl> Tables;
        public IList<int> Fields;
        public List<Database_Tbl> NewModel;
        public string TBL_Description;
        public string TBL_Name;
        public string FieldCounts;
    }
}