/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: controllers\CommChargesController.cs
 * Created by: Joel Haubold
 * Created date: 12-6-12
 * Primary Programmer: Joel Haubold
 * File description: Controller for editing/updating Committee Charges
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
	public class CommChargesController : Controller
	{
		private jashdownEntities db = new jashdownEntities();

		//
		// GET: /CommCharges/Edit/2/1

		[CommitteeSuperAdmin]
		public ActionResult Edit(int primaryKey1, int primaryKey2)
		{
			//get commCharge with newest effective date
			CommCharge commcharge = db.CommCharge.Where(cc => cc.Comm_CommOwn_ID == primaryKey1 &&
															  cc.Comm_ID == primaryKey2)
												 .OrderByDescending(cc => cc.EffectiveDate).First();
			if (commcharge == null)
			{
				return HttpNotFound();
			}

			return View(commcharge);
		}

		//
		// POST: /CommCharges/Edit/2/1

		[CommitteeSuperAdmin]
		[HttpPost]
		public ActionResult Edit(int primaryKey1, int primaryKey2, CommCharge commcharge)
		{
			//get charge from database.
			//compare for changes.
			//make new charge
			//add to database.
			if (db.CommConstitution.Any(cc => cc.EffectiveDate == commcharge.EffectiveDate))
			{
				ModelState.AddModelError("EffectiveDate", "Charges with this effective date already exist.");
				return View(commcharge);
			}
			CommCharge oldCommCharge = db.CommCharge.Where(cc => cc.Comm_CommOwn_ID == primaryKey1 &&
																 cc.Comm_ID == primaryKey2)
													.OrderByDescending(cc => cc.EffectiveDate).First();
			if (commcharge.EffectiveDate > DateTime.Now)
			{
				ModelState.AddModelError("EffectiveDate", "Effective date may not occur in the future.");
			}

			if (!ModelState.IsValid)
			{
				return View(commcharge);
			}

			if (oldCommCharge.Charges == commcharge.Charges && oldCommCharge.EffectiveDate == commcharge.EffectiveDate)
			{				//no change
				return RedirectToAction("details", "committees", new { primaryKey1, primaryKey2 });
			}
			else
			{
				CommCharge newCommCharge = new CommCharge();
				newCommCharge.Charges = commcharge.Charges;
				newCommCharge.CreatedBy = User.Identity.Name;
				newCommCharge.CreatedDate = DateTime.Today;
				newCommCharge.EffectiveDate = commcharge.EffectiveDate;
				newCommCharge.Comm_CommOwn_ID = primaryKey1;
				newCommCharge.Comm_ID = primaryKey2;
				db.CommCharge.Add(newCommCharge);
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