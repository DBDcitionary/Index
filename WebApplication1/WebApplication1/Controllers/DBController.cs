using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            return View(db.Database_Tbl.ToList());
        }

        public ActionResult TableInformation(int? dB_ID)
        {
            return View(db.Table_Tbl.Where(T => T.DB_ID == dB_ID).First());
        }

        public ActionResult FieldInformation(int? tbl_ID)
        {
            return View(db.Field_Tbl.Where(L => L.TBL_ID == tbl_ID).FirstOrDefault());
        }
    }
}