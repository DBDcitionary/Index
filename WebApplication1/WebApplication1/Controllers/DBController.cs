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

namespace WebApplication1.Controllers
{
    public class DBController : Controller
    {
        private DB_DictionaryContext db = new DB_DictionaryContext(); //Variable for Database Connection.
        // GET: DB Information
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DatabaseInformation()
        {
            //***************************************
            //Getting Database Information
            //**************************************
            try
            {
                
                var dbInfo = db.Database_Tbl.ToList();//Listing Database Information
                return View(dbInfo);//passing the view page with the results
            }
            catch (Exception exp)
            {
                return View("Error");
            }
        }

        public ActionResult TableInformation(int ? dB_ID)
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
                var t1 = tblinfo.Where(t => t.DB_ID == dB_ID);//Filtering the list based on the selected Database ID
                if (tblinfo == null)
                {
                    return HttpNotFound();
                }
                return View(t1);//passing the Table View with the filtered results
            }
            catch (Exception exp)
            {
                return View("Error");
            }
        }

        public ActionResult FieldInformation(int? tbl_ID)
        {
            //***************************************
            //Getting Field Information
            //***************************************
            try
            {
                //Condition to check the ID of the table if `is not Empty
                if (tbl_ID == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                List<Field_Tbl> fldinfo = db.Field_Tbl.ToList();//Listing fields 
                var f1 = fldinfo.Where(l => l.TBL_ID == tbl_ID);//Filtering results based on the selected TBL ID
                if (fldinfo == null)
                {
                    return HttpNotFound();
                }
                return View(f1);
            }
            catch (Exception exp)
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
                if (searchby == "1")
                {
                    //Returning results and Refined by Database information 
                    querylist.dblist = db.Database_Tbl.Where(a => a.DB_Name.StartsWith(search) || a.DB_Name == search || search == null).ToList().ToPagedList(page ?? 1, 2);//Getting database information list
                     
                    querylist.tbllist = db.Table_Tbl.Where(b => b.TBL_Name == search).ToList().ToPagedList(page ?? 1, 2);

                    querylist.fldlist = db.Field_Tbl.Where(c => c.Field_Name == search).ToList().ToPagedList(page ?? 1, 2);

                    return View(querylist);
                }
                else if (searchby == "2")
                {
                    //Returning results and Refined by Table information
                    querylist.dblist = db.Database_Tbl.Where(a => a.DB_Name == search).ToList().ToPagedList(page ?? 1, 3);

                    querylist.tbllist = db.Table_Tbl.Where(b => b.TBL_Name == search || b.TBL_Name.StartsWith(search) || search == null).ToList().ToPagedList(page ?? 1, 3);//Gettting Table information list

                    querylist.fldlist = db.Field_Tbl.Where(c => c.Field_Name == search).ToList().ToPagedList(page ?? 1, 3);
                    return View(querylist);
                }
                else if (searchby == "3")
                {
                    //Returning results and Refined by Field Information
                    querylist.dblist = db.Database_Tbl.Where(a => a.DB_Name == search).ToList().ToPagedList(page ?? 1, 3);
                    querylist.tbllist = db.Table_Tbl.Where(b => b.TBL_Name == search).ToList().ToPagedList(page ?? 1, 3);
                    querylist.fldlist = db.Field_Tbl.Where(c => c.Field_Name == search || c.Field_Name.StartsWith(search) || search == null).ToList().ToPagedList(page ?? 1, 3);//Getting Field Infromation list
                    return View(querylist);
                }
                else
                {
                    //Returning results
                    querylist.dblist = db.Database_Tbl.Where(a => a.DB_Name.StartsWith(search)).ToList().ToPagedList(page ?? 1, 10);//Getting database information list
                    querylist.tbllist = db.Table_Tbl.Where(b => b.TBL_Name.StartsWith(search)).ToList().ToPagedList(page ?? 1, 10);//Gettting Table information list
                    querylist.fldlist = db.Field_Tbl.Where(c => c.Field_Name.StartsWith(search)).ToList().ToPagedList(page ?? 1, 10);//Getting Field Infromation list
                    return View(querylist);//Passing View With List that is Contained in querylist variable
                }
            }
            catch (Exception exp)
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