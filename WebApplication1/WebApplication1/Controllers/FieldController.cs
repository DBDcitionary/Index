using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;

namespace WebApplication1.Controllers
{
    public class FieldController : Controller
    {
        private DB_DictionaryContext db = new DB_DictionaryContext();

        // GET: Field
        public ActionResult Index()
        {
            var field_Tbl = db.Field_Tbl.Include(f => f.Table_Tbl);
            return View(field_Tbl.ToList());
        }
        // GET: Field/Details
        public ActionResult Details(int? id)
        {
            Field_Tbl field_Tbl = db.Field_Tbl.Find(id);
            return View(field_Tbl);
        }
        //GET: Field/Create
        public ActionResult Create()
        {
            ViewBag.DB_ID = new SelectList(db.Database_Tbl, "DB_ID", "DB_Name");
            ViewBag.Table_ID = new SelectList(db.Table_Tbl, "TBL_Name", "TBL_Description");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Field_Name,Field_Description")] Field_Tbl field_Tbl)
        {
            try 
            {
                if (ModelState.IsValid)
                {
                    db.Field_Tbl.Add(field_Tbl);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your systems administrator.");
            }
            return View(field_Tbl);
        }
    }
}