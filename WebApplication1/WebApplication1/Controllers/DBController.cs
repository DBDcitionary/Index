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
        // GET: DB_Infor_
        public ActionResult Index()
        {
            return View(db.Database_Tbl.ToList());
        }
    }
}