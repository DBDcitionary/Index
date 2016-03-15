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
        private DB_DictionaryContext db = new DB_DictionaryContext();
      // GET: DB Information
        //[Authorize]
        public ActionResult Index(string ServerName)
        {
            try
            {
                SqlDataSourceEnumerator NetworkServers = SqlDataSourceEnumerator.Instance;
                DataTable dataSource = NetworkServers.GetDataSources();
                List<string> fullServerName = new List<string>();
                for (int i = 0; i < dataSource.Rows.Count; i++)
                {
                        DataRow Row = dataSource.Rows[i];
                        var servername = dataSource.Columns[WebConfigurationManager.AppSettings[5]].Table.Rows[i].ItemArray[0].ToString();
                        var instancename = dataSource.Columns[WebConfigurationManager.AppSettings[6]].Table.Rows[i].ItemArray[1].ToString();
                        string newServerName = servername + "\\" + instancename;
                        switch (instancename)
                        {
                            case "":
                                fullServerName.Add(servername);
                                break;
                            default:
                                fullServerName.Add(newServerName);
                                break;
                        }
                        ViewBag.ServerName = new SelectList(fullServerName);
                }

                if (dataSource.Rows.Count == 0)
                {
                    ViewBag.ServerName = new SelectList(new[]{ "No Server Found" });
                }
                else
                {
                    return View();
                }
                return View();
                
            }
            catch (Exception)
            {
                return View("ServerError");
            }  
        }

        [HttpPost]
        public ActionResult Index(string ServerName, string altServerName, bool alternative, bool authen, string uName, string pWord, string connectString,string providerName)
        {
            List<string> dbnames = new List<string>(); //variable to hold database names list
            try
            {
                //*****************************************************************
                //CONDITION TO CHECK WHICH SERVER YOU USING FROM cmbbox or txtbox
                //*****************************************************************
                switch (alternative)
                {
                    case true:
                        if (authen == true)//condition to check if user is using SQL Authentication to Login into the Server
                        {
                            connectString = WebConfigurationManager.AppSettings[7]+altServerName+WebConfigurationManager.AppSettings[9]+uName+ WebConfigurationManager.AppSettings[10]+pWord; //Untrusted Connection string
                        }
                        else
                            connectString = WebConfigurationManager.AppSettings[7]+altServerName+WebConfigurationManager.AppSettings[8]; //Trusted Connection string
                        ViewBag.Name = (altServerName);//Passing the Server Name into ViewBag Properties
                        break;
                    default:
                        if (authen == true)
                        {
                            connectString = WebConfigurationManager.AppSettings[7]+ServerName+WebConfigurationManager.AppSettings[9]+uName+WebConfigurationManager.AppSettings[10]+pWord;
                        }
                        else
                            connectString = WebConfigurationManager.AppSettings[7]+ServerName+WebConfigurationManager.AppSettings[8];
                        ViewBag.Name = (ServerName);
                        break;
                }

                //**************************
                //ADDING CONNECTION STRING
                //**************************
                Configuration constring = WebConfigurationManager.OpenWebConfiguration("~");
                int con = ConfigurationManager.ConnectionStrings.Count;
                ConnectionStringSettings conset = new ConnectionStringSettings("conn", connectString,providerName+WebConfigurationManager.AppSettings[11]);
                ConnectionStringsSection stringsec = constring.ConnectionStrings;
                stringsec.ConnectionStrings.Remove(conset);
                stringsec.ConnectionStrings.Add(conset);
                constring.Save(ConfigurationSaveMode.Modified);
                //***************************
                //USING CONNECTION STRING
                //***************************
                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT [name] FROM [master].[dbo].sysdatabases WHERE dbid > 6", conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow Row = dt.Rows[i];
                        var dbname = dt.Columns[0].Table.Rows[i].ItemArray[0].ToString();
                        dbnames.Add(dbname);
                    }

                    if (dt.Rows.Count == 0)
                    {
                        ViewBag.DBName = new SelectList("");
                    }
                }
                ViewBag.DBName = new SelectList(dbnames);
                ViewBag.tblName = new SelectList("");
                return View("DatabaseInformation");
            }
            catch (Exception)
            {
                return View("ServerError");
            }
        }

        [HttpGet]
        public ActionResult DatabaseInformation(string DBName)
        { 
            try
            {
                List<string> tbllist = new List<string>();
                List<string> dbnames = new List<string>();
                using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["conn"].ConnectionString))
                {
                    con.Open();
                    //*******************************
                    //Getting Database Information
                    //*******************************
                    SqlCommand cmd = new SqlCommand("SELECT [name] FROM [master].[dbo].sysdatabases WHERE dbid > 6", con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow Row = dt.Rows[i];
                        var dbname = dt.Columns[0].Table.Rows[i].ItemArray[0].ToString();
                        dbnames.Add(dbname);
                    }
                    //***************************
                    //Getting Table Information
                    //***************************
                    SqlCommand cmd1 = new SqlCommand("SELECT * FROM ["+DBName+"].INFORMATION_SCHEMA.TABLES", con);
                    SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                    DataTable dt1 = new DataTable();
                    da1.Fill(dt1);
                    string name = dt1.Rows[0].ItemArray[2].ToString();
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        DataRow Row = dt1.Rows[i];
                        var tblNames = dt1.Rows[i].ItemArray[2].ToString();
                        tbllist.Add(tblNames);
                    }

                    if (dt.Rows.Count == 0)
                    {
                        ViewBag.tblName = new SelectList("");
                    }
                }
                ViewBag.DBName = new SelectList(dbnames);
                ViewBag.tblName = new SelectList(tbllist);
                return View();
            }
            catch (Exception)
            {
                return View("Error");
            }
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
                List<Table_Tbl> tblinfo = db.Table_Tbl.ToList();//listing Table Information based on the selected Database ID
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
                List<Field_Tbl> fldinfo = db.Field_Tbl.ToList();//Listing fields 
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
                        querylist.dblist = db.Database_Tbl.OrderBy(c=>c.DB_Name).Where(a => a.DB_Name.Contains(search) || a.DB_Name == search || search == null).ToList().ToPagedList(page ?? 1, 20);//Getting database information list
                        if (querylist.dblist == null || querylist.dblist.Count == 0)
                            ViewBag.Mesage = "No Results Found";
                        break;
                    case "2":
                        //Returning results and Refined by Table information 
                        querylist.tbllist = db.Table_Tbl.OrderBy(c => c.TBL_Name).Where(b => b.TBL_Name == search || b.TBL_Name.Contains(search) || search == null).ToList().ToPagedList(page ?? 1, 20);//Gettting Table information list
                        if (querylist.tbllist == null || querylist.tbllist.Count == 0)
                            ViewBag.Mesage = "No Results Found";
                        break;
                    case "3":
                        //Returning results and Refined by Field information 
                        querylist.fldlist = db.Field_Tbl.OrderBy(c => c.Field_Name).Where(c => c.Field_Name == search || c.Field_Name.Contains(search) || search == null).ToList().ToPagedList(page ?? 1, 20);//Getting Field Infromation list
                        if (querylist.fldlist == null || querylist.fldlist.Count == 0)
                            ViewBag.Mesage = "No Results Found";
                        break;
                    case "4":
                        //Returning all Results
                        qlist newlist = new qlist();
                        newlist.dblist = db.Database_Tbl.Where(a => a.DB_Name.Contains(search) || search == null).ToList().ToPagedList(page ?? 1, 20);//Getting database information list
                        newlist.tbllist = db.Table_Tbl.Where(b => b.TBL_Name.Contains(search) || search == null).ToList().ToPagedList(page ?? 1, 20);//Gettting Table information list
                        newlist.fldlist = db.Field_Tbl.Where(c => c.Field_Name.Contains(search) || search == null).ToList().ToPagedList(page ?? 1, 20);//Getting Field Infromation list
                        querylist = newlist;
                        if(newlist == null)
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
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}