using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class DBController : Controller
    {
        DB_DictionaryContext db = new DB_DictionaryContext();
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
                //Condition to check the ID of the Database is not Empty
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
                //Condition to check the ID of the table is not Empty
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
    }
}