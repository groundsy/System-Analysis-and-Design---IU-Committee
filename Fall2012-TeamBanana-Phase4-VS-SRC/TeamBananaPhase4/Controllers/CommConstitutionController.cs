/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: controllers\CommChargesController.cs
 * Created by: Joel Haubold
 * Created date: 12-6-12
 * Primary Programmer: Joel Haubold
 * File description: Controller for editing/updating Committee Constitution
 * 
 * Change Log:
 * Date programmer    change
 * 
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
	public class CommConstitutionController : Controller
	{
		private jashdownEntities db = new jashdownEntities();

		//
		// GET: /CommConstitution/Edit/2/1

		[CommitteeSuperAdmin]
		public ActionResult Edit(int primaryKey1, int primaryKey2)
		{
			//get CommConstitution with newest effective date
			CommConstitution commConstitution = db.CommConstitution.Where(cc => cc.Comm_CommOwn_ID == primaryKey1 &&
															  cc.Comm_ID == primaryKey2)
												 .OrderByDescending(cc => cc.EffectiveDate).First();
			if (commConstitution == null)
			{
				//check for committee
				if (db.Comm.Find(primaryKey1, primaryKey2) == null)
				{
					return HttpNotFound();
				} 
				else
				{
					CommConstitution newCommConstitution = new CommConstitution();
					return View(newCommConstitution);
				}
			}

			return View(commConstitution);
		}

		//
		// POST: /CommConstitution/Edit/2/1

		[CommitteeSuperAdmin]
		[HttpPost]
		public ActionResult Edit(int primaryKey1, int primaryKey2, CommConstitution commconstitution)
		{
			//get charge from database.
			//compare for changes.
			//make new charge
			//add to database.
			if (db.CommConstitution.Any(cc => cc.EffectiveDate == commconstitution.EffectiveDate))
			{
				ModelState.AddModelError("EffectiveDate","A constitution with this effective date already exists.");
				return View(commconstitution);
			}
			CommConstitution oldCommCharge = db.CommConstitution.Where(cc => cc.Comm_CommOwn_ID == primaryKey1 &&
																 cc.Comm_ID == primaryKey2)
													.OrderByDescending(cc => cc.EffectiveDate).First();
			if (commconstitution.EffectiveDate > DateTime.Now)
			{
				ModelState.AddModelError("EffectiveDate", "Effective date may not occur in the future.");
			}

			if (!ModelState.IsValid)
			{
				return View(commconstitution);
			}

			if (oldCommCharge.Constitution == commconstitution.Constitution)
			{				//no change
				return RedirectToAction("details", "committees", new { primaryKey1, primaryKey2 });
			}
			else
			{
				CommConstitution newCommConstitution = new CommConstitution();
				newCommConstitution.Constitution = commconstitution.Constitution;
				newCommConstitution.CreatedBy = User.Identity.Name;
				newCommConstitution.CreatedDate = DateTime.Today;
				newCommConstitution.EffectiveDate = commconstitution.EffectiveDate;
				newCommConstitution.Comm_CommOwn_ID = primaryKey1;
				newCommConstitution.Comm_ID = primaryKey2;
				db.CommConstitution.Add(newCommConstitution);
				db.SaveChanges();
				return RedirectToAction("details", "committees", new { primaryKey1, primaryKey2 });
			}
		}

		protected override void Dispose(bool disposing)
		{
			db.Dispose();
			base.Dispose(disposing);
		}
	}
}