/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: Controllers/CommitteeController.cs
 * Created by: MVC Framework
 * Created date: 11-17-12
 * Primary Programmer: 
 * File description: Contains logic for CRUD operations on meetings 
 * 
 * Change Log:
 *  Date     Programmer      Change
 * 11-19-12     jh           Changed function paramaters to primaryKey1,primaryKey2 to deal with composite key.
 * 11-28-12     dt           Changed the HttpPost Create including add committee charge and constitution.
 * 11-28-12     dt           Changed the HttpPost Edit including add committee charge and constitution.
 * 11-29-12     dt           Changed the HttpPost DeleteConfirmed.
 * 11-30-12     js           Added Division Method for CSA
 * 12-4-12		jh			 Commented out index
 *				jh			 Made create work properly (check for duplicate committee, pass pk into view)
 * 12-4-12		dt           Specified CSA previlege for Edit and Delete HttpPost method
 * 12-5-12      dt           Perform auditlogs for creating, editting, deleting committees
 * 12-8-12		jh			 Added convener button logic
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
	public class CommitteesController : Controller
	{
		private jashdownEntities db = new jashdownEntities();

		// GET: /Committees/Divison
		/*
		 * Commented out in favor of using the Division Controller
		 * 
		[CommitteeSuperAdmin]
		public ActionResult Division(int primaryKey1 = 0)
		{
			var unitFound = db.Unit.FirstOrDefault(c => c.CommOwn_ID == primaryKey1);
			if (unitFound != null)
			{
				ViewBag.Title = unitFound.Name + " Committees";
			}

			var schoolFound = db.School.FirstOrDefault(c => c.CommOwn_ID == primaryKey1);
			if (schoolFound != null)
			{
				ViewBag.Title = schoolFound.Name + " Committees";
			}

			var campusFound = db.Campus.FirstOrDefault(c => c.CommOwn_ID == primaryKey1);
			if (campusFound != null)
			{
				ViewBag.Title = campusFound.Name + " Committees";
			}

			var universityFound = db.University.FirstOrDefault(c => c.CommOwn_ID == primaryKey1);
			if (universityFound != null)
			{
				ViewBag.Title = universityFound.Name + " Committees";
			}

			var comm = db.Comm.Where(c => c.CommOwn_ID == primaryKey1).Include(c => c.CommOwn).Include(c => c.SysUser).Include(c => c.SysUser1);
			return View(comm.ToList());
		}*/

		//
		// GET: /Committees/Details/Comm_Own_ID/ID
		// GET: /Committees/Details/5/4
		[HttpGet]
		public ActionResult Details(int primaryKey1, int primaryKey2)
		{
			Comm comm = db.Comm.Find(primaryKey1, primaryKey2);
			if (comm == null)
				return HttpNotFound();
			else
			{	//determine access levels and send to view
				if (db.CommMember.Any(cm => cm.Comm_CommOwn_ID == primaryKey1 &&
											 cm.Comm_ID == primaryKey2 &&
											 cm.Member_Email == User.Identity.Name &&
											 cm.StartDate <= DateTime.Now &&
											 DateTime.Now <= cm.EndDate))
				{
					ViewBag.isMember = true;
					ViewBag.documentList = comm.CommDocument.Where(cd => cd.IsArchived == "N")
																	.OrderBy(cd => cd.Category).Select(cd => new TeamBananaPhase4.Models.CommDocumentPKWithFilenameTags
																	{
																		Comm_CommOwn_ID = cd.Comm_CommOwn_ID,
																		Comm_ID = cd.Comm_ID,
																		Filename = cd.Filename,
																		Tags = cd.Tags,
																		IsPublic = cd.IsPublic,
																		Comm_Name = cd.Comm.Name,
																		Category = cd.Category,
                                                                        DisplayDate = cd.DisplayDate,
																		Title = cd.Title
																	}).ToList();
				}
				else
				{
					ViewBag.documentList = comm.CommDocument.Where(cd => cd.IsArchived == "N" && cd.IsPublic == "Y")
																		.OrderBy(cd => cd.Category)
																		.Select(cd => new TeamBananaPhase4.Models.CommDocumentPKWithFilenameTags
																		{
																			Comm_CommOwn_ID = cd.Comm_CommOwn_ID,
																			Comm_ID = cd.Comm_ID,
																			Filename = cd.Filename,
																			Tags = cd.Tags,
																			IsPublic = cd.IsPublic,
																			Comm_Name = cd.Comm.Name,
																			Category = cd.Category,
                                                                            DisplayDate = cd.DisplayDate,
																			Title = cd.Title
																		}).ToList();
					ViewBag.isMember = false;
				}
				if (db.CommMember.Any(cm => cm.Comm_CommOwn_ID == primaryKey1 &&
											 cm.Comm_ID == primaryKey2 &&
											 cm.Member_Email == User.Identity.Name &&
											 (cm.IsAdministrator == "Y" || cm.IsConvener == "Y") &&
											 cm.StartDate <= DateTime.Now &&
											 DateTime.Now <= cm.EndDate))
					ViewBag.isCommitteeAdmin = true;
				else
					ViewBag.isCommitteeAdmin = false;
				if (db.CommMember.Any(cm => cm.Comm_CommOwn_ID == primaryKey1 &&
											 cm.Comm_ID == primaryKey2 &&
											 cm.Member_Email == User.Identity.Name &&
											 cm.IsConvener == "Y" &&
											 cm.StartDate <= DateTime.Now &&
											 DateTime.Now <= cm.EndDate))
					ViewBag.isConvener = true;
				else
					ViewBag.isConvener = false;
				if (db.CommSuperAdmin.Any(csa => csa.CommOwn_ID == primaryKey1 &&
												 csa.SysUser_Email == User.Identity.Name &&
												 csa.StartDate <= DateTime.Today &&
												 (csa.EndDate ?? DateTime.MaxValue) >= DateTime.Today))
				{
					ViewBag.isDeleteable = !comm.CommMember.Any(); //committee is deleteable only if it has never had any members.
					ViewBag.isCSA = true;
				}
				else
					ViewBag.isCSA = false;
				return View(comm);
			}

		}
		//
		// GET: /Committees/Create
		[CommitteeSuperAdmin]
		public ActionResult Create(int primaryKey1)
		{
			Comm comm = new Comm();
			comm.CommOwn_ID = primaryKey1;
			return View(comm);
		}

		//
		// POST: /Committees/Create
		[CommitteeSuperAdmin]
		[HttpPost]
		public ActionResult Create(Comm comm, string txtCharge, string txtConstitution)
		{
			if (!db.CommSuperAdmin.Any(csa => csa.SysUser_Email == User.Identity.Name &&
											csa.StartDate <= DateTime.Today &&
											(csa.EndDate ?? DateTime.MaxValue) >= DateTime.Today &&
											csa.CommOwn_ID == comm.CommOwn_ID))
			{
				return RedirectToAction("login", "account", null);
			}

			//make sure an active committee with the same name doesn't exist for this division already.

			if (db.Comm.Any(c => c.Name == comm.Name &&
							 c.CommOwn_ID == comm.CommOwn_ID &&
							 c.IsArchived == "N"))
			{
				ModelState.AddModelError("Name", "An active committee with this name already exists for this division");
				return View(comm);
			}

			comm.CreatedDate = DateTime.Now;
			comm.CreatedBy = User.Identity.Name;
			comm.IsArchived = "N";
			
			CommCharge commCharge = new CommCharge();
			CommConstitution commCon = new CommConstitution();

			if (ModelState.IsValid)
			{
				//save new committee so we can get the new autonumber for comm.ID
				db.Comm.Add(comm);
				db.SaveChanges();
				
				// Start adding Committees Charges into the database
				commCharge.Comm_CommOwn_ID = comm.CommOwn_ID;
				commCharge.Comm_ID = comm.ID;
				commCharge.Charges = txtCharge;
				commCharge.CreatedBy = User.Identity.Name;
				commCharge.CreatedDate = DateTime.Now.Date;
				commCharge.EffectiveDate = comm.EffectiveDate;
				db.CommCharge.Add(commCharge);
				// Finish adding Committees Charges into the database

				// Start adding Committees Constitution into the database
				commCon.Comm_CommOwn_ID = comm.CommOwn_ID;
				commCon.Comm_ID = comm.ID;
				commCon.Constitution = txtConstitution;
				commCon.EffectiveDate = comm.EffectiveDate;
				commCon.CreatedDate = DateTime.Now.Date;
				commCon.CreatedBy = User.Identity.Name;
				db.CommConstitution.Add(commCon);
				// Finish adding Committees Constitution into the database
				db.SaveChanges();
				//AuditLogController.Add("Add", User.Identity.Name, "New committee name: " + comm.Name + " is added"); //Not needed since created by/date is maintained by comm
				return RedirectToAction("details", "committees", new { primaryKey1 = comm.CommOwn_ID, primaryKey2 = comm.ID });
			}

			return View(comm);
		}

		// GET: /Committees/Edit/Comm_Own_ID/ID
		// GET: /Committees/Edit/5/4
		[CommitteeSuperAdmin]
		[HttpGet]
		public ActionResult Edit(int primaryKey1 = 0, int primaryKey2 = 0)
		{
			Comm comm = db.Comm.Find(primaryKey1, primaryKey2);
			if (comm == null)
			{
				return HttpNotFound();
			}

			//get data to populate drop downs on view
			
			return View(comm);
		}

		// POST: /Committees/Edit/Comm_Own_ID/ID
		// POST: /Committees/Edit/5/4

		[CommitteeSuperAdmin]
		[HttpPost]
		public ActionResult Edit(int primaryKey1, int primaryKey2, Comm comm)
		{
			// if Check current user is Committees Super Admin first
			// If (isCSA is true)
			if (ModelState.IsValid)
			{

				db.Entry(comm).State = EntityState.Modified;
				db.SaveChanges();

				AuditLogController.Add("Edit", User.Identity.Name, "Edited committee: " + comm.Name);
				return RedirectToAction("details", new { primaryKey1 = comm.CommOwn_ID, primaryKey2 = comm.ID });
			}
			
			return View(comm);
		}

		// GET: /Committees/Delete/5/4
		[CommitteeSuperAdmin]
		public ActionResult Delete(int primaryKey1, int primaryKey2)
		{
			Comm comm = db.Comm.Find(primaryKey1, primaryKey2);
			if (comm == null)
			{
				return HttpNotFound();
			}
			ViewBag.isDeleteable = !comm.CommMember.Any();
			return View(comm);
		}

		// POST: /Committees/Edit/Comm_Own_ID/ID
		// POST: /Committees/Delete/5/4

		[CommitteeSuperAdmin]
		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(int primaryKey1, int primaryKey2)
		{
			// Check current user is CSA
			// if (user is CSA)
			Comm comm = db.Comm.Find(primaryKey1, primaryKey2);
			if (!comm.CommMember.Any())
			{
				db.Comm.Remove(comm);
				db.SaveChanges();
			}
			// Generate AuditLog on Delete committee
			AuditLogController.Add("Delete", User.Identity.Name, "Delete " + comm.Name);
			return RedirectToAction("index", "divisions", new { primaryKey1 = comm.CommOwn_ID });
		}

		// GET: /Committees/Delete/5/4
		[CommitteeSuperAdmin]
		public ActionResult Archive(int primaryKey1, int primaryKey2)
		{
			Comm comm = db.Comm.Find(primaryKey1, primaryKey2);
			if (comm == null)
			{
				return HttpNotFound();
			}
			return View(comm);
		}


		// POST: /Committees/Archive/5/4

		[CommitteeSuperAdmin]
		[HttpPost, ActionName("Archive")]
		public ActionResult ArchiveConfirmed(int primaryKey1, int primaryKey2, string archivedComments)
		{
			//archive committee
			Comm comm = db.Comm.Find(primaryKey1, primaryKey2);
			comm.IsArchived = "Y";
			comm.ArchivedBy = User.Identity.Name;
			comm.ArchivedDate = DateTime.Now;
			comm.ArchivedComments = archivedComments;
			{
				db.Entry(comm).State = EntityState.Modified;
				db.SaveChanges();
			}
			
			return RedirectToAction("index", "divisions", new { primaryKey1 = comm.CommOwn_ID });
		}

		protected override void Dispose(bool disposing)
		{
			db.Dispose();
			base.Dispose(disposing);
		}
	}
}