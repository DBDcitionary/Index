using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Models
{
    public class ListingDatabaseInforModels
    {

        public ListingDatabaseInforModels()
        {
            Servers = new SelectList(new[] { "" });
            Databases = new SelectList(new[] { "" });
            Tables = new SelectList(new[] { "" });
        }
       
        public ListingDatabaseInforModels(List<Database_Tbl> Model)
        {
            getServers(Model);
        }
          
        //Method 
        private IEnumerable<SelectListItem> getServers(List<Database_Tbl> Model)
        {
            getDatabases(Model);
            return Servers = new SelectList(Model.Select(a => a.ServerName));
        }

        private IEnumerable<SelectListItem> getDatabases(List<Database_Tbl> Model)
        {
            getTables(Model);
            return Databases = new SelectList(Model.Select(a => a.DB_Name));
        }

        private IEnumerable<SelectListItem> getTables(List<Database_Tbl> Model)
        {
            return Tables = new SelectList(Model.Select(a => a.Table_Tbl.Select(b=>b.TBL_Name)));
        }

        public IEnumerable<SelectListItem> Servers;
        public IEnumerable<SelectListItem> Databases;
        public IEnumerable<SelectListItem> Tables;
    }
}