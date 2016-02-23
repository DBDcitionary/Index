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

        public ActionResult DatabaseInformation(int? page, int ? server)
        {
            //***************************************
            //Getting Database Information
            //**************************************
            try
            {
                
                var dbInfo = db.Database_Tbl.ToList().ToPagedList(page ?? 1, 50);//Listing Database Information
                return View(dbInfo);//passing the view page with the results
            }
            catch (Exception)
            {
                return View("Error on Database Information");
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
                return View("Error on Table Information");
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
                return View("Error Field Information");
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
                return View("Error On Search");
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