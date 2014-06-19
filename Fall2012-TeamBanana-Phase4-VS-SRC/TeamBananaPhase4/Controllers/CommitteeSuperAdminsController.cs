using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamBananaPhase4.Models;

namespace TeamBananaPhase4.Controllers
{

    public class CommitteeSuperAdminsController : Controller
    {
        /* Only an IT Admin can do any of the following */
       
        private jashdownEntities db = new jashdownEntities();

        //
        // GET: /CommitteeSuperAdmins/
        /*
        [ITAdmin]
        public ActionResult Index()
        {
            var commsuperadmin = db.CommSuperAdmin.Include(c => c.CommOwn).Include(c => c.SysUser).Include(c => c.SysUser1);
            return View(commsuperadmin.ToList());
        }
        */


        //
        // GET: /CommitteeSuperAdmins/Division/primaryKey
        //[ITAdmin]
        /* public ActionResult Divison(int primaryKey1 = 0)
         {
             var unitFound = db.Unit.FirstOrDefault(c => c.CommOwn_ID == primaryKey1);
             if (unitFound != null)
             {
                 ViewBag.Title = unitFound.CommOwn_ID;
             }

             var schoolFound = db.School.FirstOrDefault(c => c.CommOwn_ID == primaryKey1);
             if (schoolFound != null)
             {
                 ViewBag.Title = schoolFound.Name;
             }

             var campusFound = db.Campus.FirstOrDefault(c => c.CommOwn_ID == primaryKey1);
             if (campusFound != null)
             {
                 ViewBag.Title = campusFound.Name;
             }

             var universityFound = db.University.FirstOrDefault(c => c.CommOwn_ID == primaryKey1);
             if (universityFound != null)
             {
                 ViewBag.Title = universityFound.Name;
             }
            
             var commsuperadmin = db.CommSuperAdmin.Include(c => c.CommOwn).Include(c => c.SysUser).Include(c => c.SysUser1);
             return View(commsuperadmin.ToList());
         }*/

        //
        // GET: /CommitteeSuperAdmins/Details/5
        /*
        [ITAdmin]
        public ActionResult Details(string primaryKey1 = "", int primaryKey2 = 0, string primaryKey3 = "")
        {
            //convert primaryKey3 into a valid dateTime object, if not valid return page not found.
            DateTime meetingPKDateTime;
            if (!DateTime.TryParse(primaryKey3, out meetingPKDateTime))
                return HttpNotFound();

            CommSuperAdmin commsuperadmin = db.CommSuperAdmin.Find(primaryKey1, primaryKey2, primaryKey3);
            if (commsuperadmin == null)
            {
                return HttpNotFound();
            }
            return View(commsuperadmin);
        }*/

        //
        // GET: /CommitteeSuperAdmins/Create
        [ITAdmin]
        public ActionResult Create(int primaryKey1)
        {
            ViewBag.CommOwn_ID = primaryKey1;
            ViewBag.SysUser_Email = new SelectList(db.SysUser, "Email", "Email");
            return View();
        }

        //
        // POST: /CommitteeSuperAdmins/Create
        [ITAdmin]
        [HttpPost]
        public ActionResult Create(int primaryKey1, CommSuperAdmin commsuperadmin)
        {
            commsuperadmin.CommOwn_ID = primaryKey1;
            commsuperadmin.CreatedBy = User.Identity.Name;
            commsuperadmin.CreatedDate = DateTime.Now;

            if (db.CommSuperAdmin.Any(CSA => CSA.CommOwn_ID == primaryKey1 &&
                                     CSA.SysUser_Email == commsuperadmin.SysUser_Email &&
                                     CSA.StartDate == commsuperadmin.StartDate))
            {
                ModelState.AddModelError("StartDate", "User already has this start date for this Division");
            }
            else if (ModelState.IsValid)
            {
                db.CommSuperAdmin.Add(commsuperadmin);
                db.SaveChanges();
                return RedirectToAction("Index", "Divisions", new {primaryKey1 = primaryKey1});
            }

            ViewBag.CommOwn_ID = primaryKey1;
            ViewBag.SysUser_Email = new SelectList(db.SysUser, "Email", "Email");
            return View(commsuperadmin);
        }

        //
        // GET: /CommitteeSuperAdmins/Edit/user@iusb.edu/5/
        [ITAdmin]
        public ActionResult Edit(string primaryKey1 = "", int primaryKey2 = 0, string primaryKey3 = "")
        {
            //convert primaryKey3 into a valid dateTime object, if not valid return page not found.
            DateTime meetingPKDateTime;
            if (!DateTime.TryParse(primaryKey3, out meetingPKDateTime))
                return HttpNotFound();
            CommSuperAdmin commsuperadmin = db.CommSuperAdmin.Find(primaryKey1, primaryKey2, meetingPKDateTime);

            if (commsuperadmin == null)
            {
                return HttpNotFound();
            }
            return View(commsuperadmin);
        }

        //
        // POST: /CommitteeSuperAdmins/Edit/5
        [ITAdmin]
        [HttpPost]
        public ActionResult Edit(CommSuperAdmin commsuperadmin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(commsuperadmin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Divisions", new { primaryKey1 = commsuperadmin.CommOwn_ID });
            }

            return View(commsuperadmin);
        }

        //
        // GET: /CommitteeSuperAdmins/Delete/5
        /*
        [ITAdmin]
        public ActionResult Delete(string id = null)
        {
            CommSuperAdmin commsuperadmin = db.CommSuperAdmin.Find(id);
            if (commsuperadmin == null)
            {
                return HttpNotFound();
            }
            return View(commsuperadmin);
        }*/

        //
        // POST: /CommitteeSuperAdmins/Delete/5
        /*
        [ITAdmin]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            CommSuperAdmin commsuperadmin = db.CommSuperAdmin.Find(id);
            db.CommSuperAdmin.Remove(commsuperadmin);
            db.SaveChanges();
            return RedirectToAction("Index");
        }*/

        [ITAdmin]
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}