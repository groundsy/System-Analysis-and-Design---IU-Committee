/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: Controllers/CommMemberController.cs
 * Created by: MVC Framework
 * Created date: 11-28-12
 * Primary Programmer: 
 * File description: Contains logic for CRUD operations on meetings 
 * 
 * Change Log:
 *  Date     Programmer      Change
 * 11-28-12     dt           Added primarykey1,primarykey2,primarykey3, and primarykey4.
 * 11-29-12     dt           Edited HttpPost Create action and add 4 primary keys in HttpPost Edit action.
 * 
*************************************************/

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
    public class CommMemberController : Controller
    {
        private jashdownEntities db = new jashdownEntities();

        //
        // GET: /CommMember/
        /*
        public ActionResult Index()
        {
            var commmember = db.CommMember.Include(c => c.Comm).Include(c => c.CommOwn).Include(c => c.MemberRole).Include(c => c.SysUser).Include(c => c.SysUser1);
            return View(commmember.ToList());
        }*/

        //
        // GET: /CommMember/Details/5
        /*
        public ActionResult Details(int primaryKey1 = 0, int primaryKey2 = 0, string primaryKey3 = "", string primaryKey4 = "")
        {

            DateTime comMemberPKDateTime;
            primaryKey3 = HttpUtility.UrlDecode(primaryKey3);
            if (!DateTime.TryParse(primaryKey4, out comMemberPKDateTime))
                return HttpNotFound();

            CommMember commmember = db.CommMember.Find(primaryKey1, primaryKey2, primaryKey3, comMemberPKDateTime);
            if (commmember == null)
                return HttpNotFound();

            return View(commmember);
        }*/

        //
        // GET: /CommMember/Create
        [CommitteeSuperAdmin]
        public ActionResult Create(int primaryKey1, int primaryKey2)
        {
            ViewBag.Comm_CommOwn_ID = primaryKey1;
            ViewBag.Comm_ID = primaryKey2;
            Comm comm = db.Comm.Find(primaryKey1, primaryKey2);
            ViewBag.CommName = comm.Name;
            ViewBag.MemberRoles = new SelectList(db.MemberRole, "Role", "Role");
            //ViewBag.Representing = new SelectList(db.CommOwn, "ID", "ID");
            ViewBag.Member_Email = new SelectList(db.SysUser, "Email", "Email");
            return View();
        }

        //
        // POST: /CommMember/Create
        [CommitteeSuperAdmin]
        [HttpPost]
        public ActionResult Create(int primaryKey1, int primaryKey2, CommMember commmember)
        {
            ViewBag.Comm_CommOwn_ID = primaryKey1;
            ViewBag.Comm_ID = primaryKey2;
            Comm comm = db.Comm.Find(primaryKey1, primaryKey2);
            ViewBag.CommName = comm.Name;
			
            //Set Variables
            commmember.Comm_CommOwn_ID = primaryKey1;
            commmember.Comm_ID = primaryKey2;
            commmember.LastAssignedBy = User.Identity.Name;
            commmember.LastAssignedDate = DateTime.Now;
			commmember.StartDate = commmember.StartDate.Date;
			commmember.EndDate = commmember.EndDate.Date;
			commmember.Representing = db.SysUser.Find(commmember.Member_Email).Employer_ID;
            switch (commmember.MemberRole_Role)
            {
                case "Chair":
                    commmember.IsAdministrator = "Y";
                    commmember.IsConvener = "N";
                    break;
                case "Co-chair":
                    commmember.IsAdministrator = "Y";
                    commmember.IsConvener = "N";
                    break;
                case "Convener":
                    commmember.IsConvener = "Y";
                    commmember.IsAdministrator = "N";
                    break;
                case "Member":
                    commmember.IsAdministrator = "N";
                    commmember.IsConvener = "N";
                    break;
            }

            /* TODO */
            //Determine Permissions Based on Roles

            if (ModelState.IsValid)
            {
                db.CommMember.Add(commmember);
                db.SaveChanges();
				return RedirectToAction("Details", "Committees", new { primaryKey1 = primaryKey1, primaryKey2 = primaryKey2 });
            }

            ViewBag.MemberRoles = new SelectList(db.MemberRole, "Role", "Role");
            //ViewBag.Representing = new SelectList(db.CommOwn, "ID", "ID");
            ViewBag.Member_Email = new SelectList(db.SysUser, "Email", "Email");
            return View(commmember);
        }

        //
        // GET: /CommMember/Edit/5
        [CommitteeSuperAdmin]
        public ActionResult Edit(int primaryKey1, int primaryKey2, string primaryKey3, string primaryKey4 ="")
        {


            DateTime startPKDateTime;

            if (!DateTime.TryParse(primaryKey4, out startPKDateTime))
                return HttpNotFound();

            CommMember commmember = db.CommMember.Find(primaryKey1,primaryKey2,primaryKey3,startPKDateTime);

            if (commmember == null)
            {
                return HttpNotFound();
            }

            /*
            ViewBag.Comm_CommOwn_ID = new SelectList(db.Comm, "CommOwn_ID", "Name", commmember.Comm_CommOwn_ID);
            ViewBag.Representing = new SelectList(db.CommOwn, "ID", "ID", commmember.Representing);
            ViewBag.MemberRole_Role = new SelectList(db.MemberRole, "Role", "Description", commmember.MemberRole_Role);
            ViewBag.LastAssignedBy = new SelectList(db.SysUser, "Email", "FirstName", commmember.LastAssignedBy);
            ViewBag.Member_Email = new SelectList(db.SysUser, "Email", "FirstName", commmember.Member_Email);
             */

            Comm comm = db.Comm.Find(primaryKey1, primaryKey2);
            ViewBag.CommName = comm.Name;
            ViewBag.MemberRoles = new SelectList(db.MemberRole, "Role", "Role", commmember.MemberRole_Role);

            return View(commmember);
        }

        //
        // POST: /CommMember/Edit/5
        [CommitteeSuperAdmin]
        [HttpPost]
        public ActionResult Edit(int primaryKey1,int primaryKey2, CommMember commmember)
        {

            //Set Variables
            commmember.Comm_CommOwn_ID = primaryKey1;
            commmember.Comm_ID = primaryKey2;
            commmember.LastAssignedBy = User.Identity.Name;
            commmember.LastAssignedDate = DateTime.Now;
			commmember.EndDate = commmember.EndDate.Date;
			
			switch (commmember.MemberRole_Role)
            {
                case "Chair":
                    commmember.IsAdministrator = "Y";
                    commmember.IsConvener = "N";
                    break;
                case "Co-chair":
                    commmember.IsAdministrator = "Y";
                    commmember.IsConvener = "N";
                    break;
                case "Convener":
                    commmember.IsConvener = "Y";
                    commmember.IsAdministrator = "N";
                    break;
                case "Member":
                    commmember.IsAdministrator = "N";
                    commmember.IsConvener = "N";
                    break;
            }

            if (ModelState.IsValid)
            {
				
				db.Entry(commmember).State = EntityState.Modified;
				             
				db.SaveChanges();
				
                return RedirectToAction("Details", "Committees", new { primaryKey1 = primaryKey1, primaryKey2 = primaryKey2});
            }

            Comm comm = db.Comm.Find(primaryKey1, primaryKey2);
            ViewBag.CommName = comm.Name;
            ViewBag.MemberRoles = new SelectList(db.MemberRole, "Role", "Role", commmember.MemberRole_Role);

            return View(commmember);
        }

        //
        // GET: /CommMember/Delete/5

        public ActionResult Delete(int id = 0)
        {
            CommMember commmember = db.CommMember.Find(id);
            if (commmember == null)
            {
                return HttpNotFound();
            }
            return View(commmember);
        }

        //
        // POST: /CommMember/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CommMember commmember = db.CommMember.Find(id);
            db.CommMember.Remove(commmember);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
		
		// GET: /CommMember/Convener/2/1
		[CommitteeConvener]
		public ActionResult Convener(int primaryKey1, int primaryKey2)
		{
			ViewBag.Comm_CommOwn_ID = primaryKey1;
			ViewBag.Comm_ID = primaryKey2;
			Comm comm = db.Comm.Find(primaryKey1, primaryKey2);

			ViewBag.membersList = new MultiSelectList(comm.CommMember.Where(cm => cm.StartDate <= DateTime.Today &&
																				  cm.EndDate >= DateTime.Today), "Member_Email", "Member_Email");
			ViewBag.VoteTypes = db.VoteType.ToList();
			ViewBag.CommName = comm.Name;
			//ViewBag.Representing = new SelectList(db.CommOwn, "ID", "ID");
			return View();
		}

		// Post: /CommMember/Convener/2/1
		[CommitteeConvener]
		[HttpPost]
		public ActionResult Convener(int primaryKey1, int primaryKey2, string[] newChairs)
		{
			Comm comm = db.Comm.Find(primaryKey1, primaryKey2);
			if (newChairs == null)
			{ //bad input 
				ViewBag.Error = "No members selected";
				ViewBag.Comm_CommOwn_ID = primaryKey1;
				ViewBag.Comm_ID = primaryKey2;
				

				ViewBag.membersList = new MultiSelectList(comm.CommMember.Where(cm => cm.StartDate <= DateTime.Today &&
																					  cm.EndDate >= DateTime.Today), "Member_Email", "Member_Email");
				ViewBag.VoteTypes = db.VoteType.ToList();
				ViewBag.CommName = comm.Name;
				//ViewBag.Representing = new SelectList(db.CommOwn, "ID", "ID");
				return View();
			}
			else
			{	//good input
				var CommMembers = comm.CommMember.Where(cm => cm.StartDate <= DateTime.Today &&
															  cm.EndDate >= DateTime.Today);
				string newRole;
				if (newChairs.Count() > 1)
					newRole = "Co-Chair";
				else
					newRole = "Chair";

				foreach (CommMember cm in CommMembers)
				{
					if (cm.Member_Email == User.Identity.Name)
					{
						cm.MemberRole_Role = "Member";
						cm.IsConvener = "N";
					}

					if (newChairs.Contains(cm.Member_Email))
					{
						cm.MemberRole_Role = newRole;
						cm.IsAdministrator = "Y";
						
						db.Entry(cm).State = EntityState.Modified;
					}
				}
				db.SaveChanges();
				return RedirectToAction("details", "committees", new { primaryKey1, primaryKey2 } );
			}
			
		}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}