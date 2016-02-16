using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1;

namespace WebApplication1.Controllers
{
    public class tblController : Controller
    {
        private DB_DictionaryContext db = new DB_DictionaryContext();

        // GET: tbl
        public ActionResult tblInformation()
        {
            var table_Tbl = db.Table_Tbl.Include(t => t.Database_Tbl);
            return View(table_Tbl.ToList());
        }

        // GET: tbl/Details/5
        public ActionResult Details(int? id)
        {
            Table_Tbl table_Tbl = db.Table_Tbl.Find(id);
            return View(table_Tbl);
        }

        //GET: tbl/Create
        public ActionResult Create()
        {
            ViewBag.DB_ID = new SelectList(db.Database_Tbl, "DB_ID", "DB_Name");
            return View();
        }

       [HttpPost]
       [ValidateAntiForgeryToken]
       public ActionResult Create([Bind(Include = "TBL_Name,TBL_Description")] Table_Tbl table_Tbl) 
       {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Table_Tbl.Add(table_Tbl);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your systems administrator.");
            }
          return View(table_Tbl);
        }

        //// GET: tbl/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var Table_TblToUpdate = db.Table_Tbl.Find(id);
            if (TryUpdateModel(Table_TblToUpdate, "", new string[] { "Tbl_Name", "Tbl_Description" }))
            {
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the Error
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(Table_TblToUpdate);
        }


           

        //// GET: tbl/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Table_Tbl table_Tbl = db.Table_Tbl.Find(id);
        //    if (table_Tbl == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(table_Tbl);
        //}

        //// POST: tbl/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Table_Tbl table_Tbl = db.Table_Tbl.Find(id);
        //    db.Table_Tbl.Remove(table_Tbl);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
