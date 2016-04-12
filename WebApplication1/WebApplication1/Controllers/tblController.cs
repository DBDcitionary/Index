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
        //private DB_DictionaryContext db = new DB_DictionaryContext();

        //// GET: tbl
        //public ActionResult tblInformation()
        //{
        //    var table_Tbl = db.Table_Tbl.Include(t => t.Database_Tbl);
        //    return View(table_Tbl.ToList());
        //}

        // GET: tbl/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Table_Tbl table_Tbl = db.Table_Tbl.Find(id);
            if (table_Tbl == null)
            {
                return HttpNotFound();
            }
            return View(table_Tbl);
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
