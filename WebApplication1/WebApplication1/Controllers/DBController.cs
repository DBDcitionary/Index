using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Data.Entity;
using PagedList;
using PagedList.Mvc;
using System.Web.Configuration;
using System.Configuration;
using System.Data.Sql;
using System.Data;
using System.Data.SqlClient;

namespace WebApplication1.Controllers
{
    public class DBController : Controller
    {
        private DB_DictionaryContext Context_ = new DB_DictionaryContext();
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                NetworkPaths NetworkServers = new NetworkPaths();
                DatabaseMetaData Database = new DatabaseMetaData();
                Database.NewDatabaseList = new List<DatabaseMetaData>();
                Database.NewDatabaseList = TempData["Data"] as List<DatabaseMetaData>;
                List<string> DatabaseNames = new List<string>(); 
                if (Database.NewDatabaseList != null)
                {
                    foreach(var item in Database.NewDatabaseList.Select(a=>a.DatabaseName).AsEnumerable())
                    {
                        DatabaseNames.Add(item);
                    }
                }
                ViewBag.Databases = new SelectList(DatabaseNames);
                ViewBag.ServerName = NetworkServers.ServerPathList.Items != null ? NetworkServers.ServerPathList : new SelectList(new[] { "No Server(s) Found" });
                return View("Index");
            }
            catch (Exception)
            {
                return View("ServerError");
            }
        }

        [HttpPost]
        public ActionResult Index(string ServerName, string AlternativeServerName, string UserName, string Password,string button,string Databases)
        {
            bool alternativeChecked = Request.Form["AlternativeServerClicked"] != "false";
            bool SQLAuthenticationCheked = Request.Form["SQLAuthentication"] != "false";
            bool import = (button == "Import/Update");
            ConnectionString setConnection = new ConnectionString();
            

            //Condition to Check for Which button is clicked.
            if (import == false)
            {
               // ConnectionString setConnection = new ConnectionString();
                switch (SQLAuthenticationCheked)
                {
                    case true:
                        setConnection.SQLAuthentication(ServerName, AlternativeServerName, UserName, Password, alternativeChecked);
                        break;
                    default:
                        setConnection.WindowsAuthentication(alternativeChecked, ServerName, AlternativeServerName);
                        break;
                }

                //Selecting DatabaseInfor from Selected server.
                ImportDatabaseMetadata CollectingDatabaseNames = new ImportDatabaseMetadata(setConnection.connectionString_);
                TempData["Data"] = CollectingDatabaseNames.NewDatabaseList;
                TempData["Conn"] = setConnection.connectionString_;
                return RedirectToAction("Index");
            }
            else
            {
                ImportingModel DatabaseInformation = new ImportingModel(TempData["Conn"].ToString(), Databases);
                return RedirectToAction("DatabaseInformation");
               
            }  
        }

        //[HttpGet]
        [Authorize]
        public ActionResult DatabaseInformation(string DBDescrip,string TBLDescrip,string fldDescrip,string DBName,string tblName, string server = "",string button = "",string value ="")
        {
            //******************************
            //UPDATING DATABASE DESCRIPTION
            //******************************
            bool save = (button == "Save");
            if (save)
            {
                switch(fldDescrip)
                {
                    case null:
                        if(DBDescrip == null)
                        {
                            var DBID = new DB_DictionaryContext().Database_Tbl.FirstOrDefault(a => a.DB_Name == DBName && a.ServerName == server).DB_ID.ToString();
                            int id = int.Parse(DBID);
                            var FKID = new DB_DictionaryContext().Table_Tbl.Where(a => a.DB_ID == id && a.TBL_Name == tblName).Select(a => a.FK_Tableid).FirstOrDefault();
                            IList<Table_Tbl> updatetable = new tableUpdate().updatetable(tblName, DBID, FKID, TBLDescrip);
                        }
                        else
                        {
                            IList<Database_Tbl> UpdateDatabase = new UpdateDatabaseModel().UpdateDatabase(DBName,server,DBDescrip);
                        }
                        break;
                    default:
                            //IList<Field_Tbl> UpdateField = new UpdateFieldModel().UpdateField(fldDescrip, DBName, tblName);
                        break;
                }
            }

            //*******************************
            //LISTING DATABASE INFORMATION
            //*******************************
            var DB = new DB_DictionaryContext().Database_Tbl.ToList();
            var serverList = (DB.Select(a =>a.ServerName)).Distinct().ToList();
            ViewBag.server = new SelectList(serverList);
            ViewBag.DBName = new SelectList(DB.Where(a => a.ServerName == server).Select(a => a.DB_Name));

            if (DBName == null)
            {
               ViewBag.Model = "";
            }
            else
            {
                var variable = DB.Where(b => b.DB_Name == DBName && b.ServerName == b.ServerName).ToList();
                ViewBag.Model = variable;
            }            

            //***************************
            //LISTING TABLE and FIELD INFORMATION
            //***************************
            var dbid = new DB_DictionaryContext().Database_Tbl.Where(a => a.DB_Name == DBName || a.DB_Name == null).Select(a => a.DB_ID).FirstOrDefault().ToString();
            var TBL = new DB_DictionaryContext().Table_Tbl.ToList();
            ViewBag.tblName = new SelectList(TBL.Where(b => b.DB_ID == int.Parse(dbid) || b.DB_ID == 0).Select(b => b.TBL_Name));
            if(tblName == null)
            {
                ViewBag.TBLModel = "";
            }
            else
            {
                int databID = int.Parse(dbid);
                ViewBag.TBLModel = TBL.Where(a => a.DB_ID == databID && a.TBL_Name == tblName).ToList();               
            }
            return View(ViewBag.Model);
        }

        [HttpPost]
        public ActionResult DatabaseInformation(string DBDescrip, string TBLDescrip, string fldDescrip, string _DatabName, string _TableName, string _FieldName, string server)
        {
            IList<Field_Tbl> UpdateField = new UpdateFieldModel().UpdateField(fldDescrip, _DatabName, _TableName, _FieldName);
            return RedirectToAction("DatabaseInformation", "DB");
        }

        public ActionResult TableInformation(int ? dB_ID, int? page)
        {
            //***************************************
            //Getting Table Information
            //***************************************
            try
            {
                //Condition to check the ID of the Database if is not Empty
                if (dB_ID == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                List<Table_Tbl> tblinfo = Context_.Table_Tbl.ToList();//listing Table Information based on the selected Database ID
                var t1 = tblinfo.Where(t => t.DB_ID == dB_ID).ToPagedList(page ?? 1, 50);//Filtering the list based on the selected Database ID
                if (tblinfo == null)
                {
                    return HttpNotFound();
                }
                return View(t1);//passing the Table View with the filtered results
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public ActionResult FieldInformation(int? tbl_ID, int ? page)
        {
            //***************************************
            //Getting Field Information
            //***************************************
            try
            {
                //Condition to check the ID of the table if is not Empty
                if (tbl_ID == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                List<Field_Tbl> fldinfo = Context_.Field_Tbl.ToList();//Listing fields 
                var f1 = fldinfo.Where(l => l.TBL_ID == tbl_ID).ToPagedList(page ?? 1, 50);//Filtering results based on the selected TBL ID
                if (fldinfo == null)
                {
                    return HttpNotFound();
                }
                return View(f1);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult Search(string searchby, string search,int ? page)
        {
            qlist querylist = new qlist();//Variable
            try
            {
                ViewBag.Message = null;
                switch (searchby)
                {
                    case "1":
                        //Returning results and Refined by Database information
                        querylist.dblist = Context_.Database_Tbl.OrderBy(c=>c.DB_Name).Where(a => a.DB_Name.Contains(search) || a.DB_Name == search || search == null).ToList().ToPagedList(page ?? 1, 20);//Getting database information list
                        if (querylist.dblist == null || querylist.dblist.Count == 0)
                            ViewBag.Mesage = "No Results Found";
                        break;
                    case "2":
                        //Returning results and Refined by Table information 
                          querylist.tbllist = Context_.Table_Tbl.OrderBy(c => c.TBL_Name).Where(b => b.TBL_Name == search || b.TBL_Name.Contains(search) || search == null).ToList().ToPagedList(page ?? 1, 20);//Gettting Table information list
                        if (querylist.tbllist == null || querylist.tbllist.Count == 0)
                            ViewBag.Mesage = "No Results Found";
                        break;
                    case "3":
                        //Returning results and Refined by Field information 
                        ViewBag.Max = WebConfigurationManager.AppSettings["ColMax"];
                        querylist.fldlist = Context_.Field_Tbl.OrderBy(c => c.Field_Name).Where(c => c.Field_Name == search || c.Field_Name.Contains(search) || search == null).ToList().ToPagedList(page ?? 1, 20);//Getting Field Infromation list
                        if (querylist.fldlist == null || querylist.fldlist.Count == 0)
                            ViewBag.Mesage = "No Results Found";
                        break;
                    case "4":
                        //Returning all Results
                        ViewBag.Max = WebConfigurationManager.AppSettings["ColMax"];
                        qlist newlist = new qlist();
                        newlist.dblist = Context_.Database_Tbl.Where(a => a.DB_Name.Contains(search) || search == null).ToList().ToPagedList(page ?? 1, 20);//Getting database information list
                        newlist.tbllist = Context_.Table_Tbl.Where(b => b.TBL_Name.Contains(search) || search == null).ToList().ToPagedList(page ?? 1, 20);//Gettting Table information list
                        newlist.fldlist = Context_.Field_Tbl.Where(c => c.Field_Name.Contains(search) || search == null).ToList().ToPagedList(page ?? 1, 20);//Getting Field Infromation list
                        querylist = newlist;
                        if(newlist.dblist.Count == 0 && newlist.tbllist.Count == 0 && newlist.fldlist.Count == 0)
                        ViewBag.Mesage = "No Results Found";
                        break;
                    default:
                        ViewBag.Mesage = "No Results Found";
                        break;
                }
                return View(querylist);
            }
            catch (Exception)
            {
                return View("Error");
            }
     
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context_.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}