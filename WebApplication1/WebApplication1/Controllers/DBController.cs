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

        public ActionResult TableInformation()
        {
            return View(db.Table_Tbl.ToList());
        }

        public ActionResult FieldInformation()
        {
            return View(db.Field_Tbl.ToList());
        }
    }
}