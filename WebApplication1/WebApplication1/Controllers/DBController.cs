using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Data.Entity;
using System.Data;

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
                throw exp;
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
                throw exp;
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
                throw exp;
            }
        }

        [HttpGet]
        public ActionResult Search(string searchby, string search)
        {
            qlist querylist = new qlist();//Variable
            try
            {
                if (searchby == "1")
                {
                    //Returning results and Refined by Database information 
                    querylist.dblist = db.Database_Tbl.Where(a => a.DB_Name.StartsWith(search) || a.DB_Name == search || search == null).ToList();//Getting database information list 
                    querylist.tbllist = db.Table_Tbl.Where(b => b.TBL_Name == search).ToList();
                    querylist.fldlist = db.Field_Tbl.Where(c => c.Field_Name == search).ToList();
                    return View(querylist);
                }
                else if (searchby == "2")
                {
                    //Returning results and Refined by Table information
                    querylist.dblist = db.Database_Tbl.Where(a => a.DB_Name == search).ToList();
                    querylist.tbllist = db.Table_Tbl.Where(b => b.TBL_Name == search || b.TBL_Name.StartsWith(search) || search == null).ToList();//Gettting Table information list
                    querylist.fldlist = db.Field_Tbl.Where(c => c.Field_Name == search).ToList();
                    return View(querylist);
                }
                else if (searchby == "3")
                {
                    //Returning results and Refined by Field Information
                    querylist.dblist = db.Database_Tbl.Where(a => a.DB_Name == search).ToList();
                    querylist.tbllist = db.Table_Tbl.Where(b => b.TBL_Name == search).ToList();
                    querylist.fldlist = db.Field_Tbl.Where(c => c.Field_Name == search || c.Field_Name.StartsWith(search) || search == null).ToList();//Getting Field Infromation list
                    return View(querylist);
                }
                else
                {
                    //Returning results
                    querylist.dblist = db.Database_Tbl.Where(a => a.DB_Name.StartsWith(search)).ToList();//Getting database information list
                    querylist.tbllist = db.Table_Tbl.Where(b => b.TBL_Name.StartsWith(search)).ToList();//Gettting Table information list
                    querylist.fldlist = db.Field_Tbl.Where(c => c.Field_Name.StartsWith(search)).ToList();//Getting Field Infromation list
                    return View(querylist);//Passing View With List that is Contained in querylist variable
                }
            }
            catch (Exception exp)
            {
                throw exp;
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

        //GET: DB/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DB_Name,DB_Description")] Database_Tbl database_Tbl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Database_Tbl.Add(database_Tbl);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                //Log the error.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your systems administrator.");
            }
            return View(database_Tbl);
        }

        public ActionResult Edit(int id)
        {
            Database_Tbl database_Tbl = db.Database_Tbl.Find(id);
            return View(database_Tbl);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include ="DB_Name,DB_Description")] Database_Tbl database_Tbl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(database_Tbl).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch(DataMisalignedException /*dex */)
            {
                //Log the error.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your systems administrator");
            }
            return View(database_Tbl);
        }
    }
}