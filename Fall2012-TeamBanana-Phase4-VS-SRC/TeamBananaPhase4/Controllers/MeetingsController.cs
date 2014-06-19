/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: Controllers/MeetingsController
 * Created by: MVC Framework
 * Created date: 11-17-12
 * Primary Programmer: Joel
 * File description: Contains logic for CRUD operations on meetings 
 * 
 * Change Log:
 * Date programmer    change
 * 11-22-12		Joel	All the logic should be good, except there is no permission checking.
 * 12-7-12		Joel	Added permission checking and audit logs
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
	public class MeetingsController : Controller
	{

		private jashdownEntities db = new jashdownEntities();
		
		[MeetingMember] //check if user is authorized for meeting
		public ActionResult Details(int primaryKey1 = 0, int primaryKey2 = 0, string primaryKey3 = "")
		{
			//convert primaryKey3 into a valid dateTime object, if not valid return page not found.
			DateTime meetingPKDateTime;
			if (!DateTime.TryParse(primaryKey3, out meetingPKDateTime))
				return HttpNotFound();

			//get meeting from database
			Meeting meeting = db.Meeting.Find(primaryKey1, primaryKey2, meetingPKDateTime);
			if (meeting == null)
			{
				return HttpNotFound();
			}

			//determine if current user is a current CA of meetings and send result to view
			if (meeting.Comm.CommMember.Any(cm => cm.Member_Email == User.Identity.Name &&
												 (cm.IsAdministrator == "Y" || cm.IsConvener == "Y") &&
												  cm.StartDate <= DateTime.Today &&
												  cm.EndDate >= DateTime.Today))
			{
				ViewBag.isCommitteeAdmin = true;
			}
			else
			{
				ViewBag.isCommitteeAdmin = false;
			}

			//send meeting to view
			return View(meeting);
		}

		// Receives primary key of Committee that the new meeting will belong to
		// GET: /Meetings/Create/5/4
        [CommitteeAdmin]
		public ActionResult Create(int primaryKey1, int primaryKey2)
		{
			//find committee that this new meeting will belong to
			Comm comm = db.Comm.Find(primaryKey1, primaryKey2);
			if (comm == null)
			{
				return HttpNotFound();
			}
			//TODO: check for permissions for adding committees to meetings
			
			//create a new meeting and add primary key and automatic attributes, created date is set when saving
			Meeting meeting = new Meeting();
			meeting.Comm_CommOwn_ID = primaryKey1;
			meeting.Comm_ID = primaryKey2;
			meeting.DateTime = DateTime.Now.Date;
			meeting.CreatedBy = User.Identity.Name;
			
			//ViewBag.Comm_CommOwn_ID = new SelectList(db.Comm, "CommOwn_ID", "Name");
			//ViewBag.CreatedBy = new SelectList(db.SysUser, "Email", "FirstName");

			ViewBag.CommitteeName = comm.Name; //send back name for view to display
			return View(meeting);
		}

		//
		// POST: /Meetings/Create

		[CommitteeAdmin]
		[HttpPost]
		public ActionResult Create(Meeting meeting)
		{
			//TODO: check for permissions

			//set created datetime to current datetime
			meeting.CreatedDate = DateTime.Now;

			//make sure no duplicate meeting exists.
			if (db.Meeting.Find(meeting.Comm_CommOwn_ID, meeting.Comm_ID, meeting.DateTime) != null)
			{
				ModelState.AddModelError("DateTime", "A meeting already exists at this date and time");
			}
			else
			{
				//if data is valid save and return to meeting parent
				if (ModelState.IsValid)
				{
					db.Meeting.Add(meeting);
					db.SaveChanges();
					return RedirectToAction("Details", "Meetings", new { primaryKey1 = meeting.Comm_CommOwn_ID, primaryKey2 = meeting.Comm_ID, primaryKey3 = meeting.DateTime.ToString("MM-dd-yyyy h.mm.ss t\\M") });

				}
			}
			//data is invalid so we return to the create page
			ViewBag.CommitteeName = db.Comm.Find(meeting.Comm_CommOwn_ID, meeting.Comm_ID).Name; //send committee name back name for view to display
			return View(meeting);
		}

		//
		// GET: /Meetings/Edit/5/4/12-14-2015
		[MeetingAdmin]
		public ActionResult Edit(int primaryKey1 = 0, int primaryKey2 = 0, string primaryKey3 = "")
		{
			//convert primaryKey3 into a valid dateTime object, if not valid return page not found.
			DateTime meetingPKDateTime;
			if (!DateTime.TryParse(primaryKey3, out meetingPKDateTime))
				return HttpNotFound();

			//get meeting from database
			Meeting meeting = db.Meeting.Find(primaryKey1, primaryKey2, meetingPKDateTime);
			if (meeting == null)
			{
				return HttpNotFound();
			}
			//TODO: check for permissions

			return View(meeting);
		}

		//
		// POST: /Meetings/Edit/5/4/12-14-2015

		[HttpPost]
		[MeetingAdmin]
		public ActionResult Edit(Meeting meeting)
		{
			//TODO: check permissions

			//if updated data is valid save and redirect to meeting parent committee
			if (ModelState.IsValid)
			{
				db.Entry(meeting).State = EntityState.Modified;
				db.SaveChanges();
				AuditLogController.Add("Edit",User.Identity.Name, "Edited Meeting: " + meeting.Comm_CommOwn_ID + "/" + meeting.Comm_ID + "/" + meeting.DateTime.ToString("MM-dd-yyyy h.mm.ss t\\M"));
				return RedirectToAction("Details", "Meetings", new { primaryKey1 = meeting.Comm_CommOwn_ID, primaryKey2 = meeting.Comm_ID, primaryKey3 = meeting.DateTime.ToString("MM-dd-yyyy h.mm.ss t\\M") });
			}
			
			//otherwise return to editing meeting
			return View(meeting);
		}

		//
		// GET: /Meetings/Delete/5/4/12-26-2012
		[MeetingAdmin]
		public ActionResult Delete(int primaryKey1, int primaryKey2, string primaryKey3)
		{
			//make sure date is in the correct format and is a valid date
			DateTime meetingPKDateTime;
			if (!DateTime.TryParse(primaryKey3, out meetingPKDateTime))
				return HttpNotFound();

			//find the meeting return error if it doesn't exist
			Meeting meeting = db.Meeting.Find(primaryKey1, primaryKey2, meetingPKDateTime);
			if (meeting == null)
			{
				return HttpNotFound();
			}

			//TODO: check for permissions.

			return View(meeting);
		}

		//
		// POST: /Meetings/Delete/5/5/12-26-2012

		[HttpPost, ActionName("Delete")]
		[MeetingAdmin]
		public ActionResult DeleteConfirmed(int primaryKey1, int primaryKey2, string primaryKey3, string primaryKey4)
		{
			//make sure date is in the correct format
			DateTime meetingPKDateTime;
			if (!DateTime.TryParse(primaryKey3, out meetingPKDateTime))
				return HttpNotFound();

			//find meeting
			Meeting meeting = db.Meeting.Find(primaryKey1, primaryKey2, meetingPKDateTime);

			//if meeting doesn't exist return error
			if (meeting == null)
			{
				return HttpNotFound();
			}
			//TODO : add check for permissions

			//delete meeting and return to committee
			db.Meeting.Remove(meeting);

			db.SaveChanges();
			AuditLogController.Add("Deleted",User.Identity.Name, "Deleted Meeting: " + meeting.Comm_CommOwn_ID + "/" + meeting.Comm_ID + "/" + meeting.DateTime.ToString("MM-dd-yyyy h.mm.ss t\\M"));
			return RedirectToAction("Details", "Committees", new { primaryKey1 = meeting.Comm_CommOwn_ID, primaryKey2 = meeting.Comm_ID } );
		}

		protected override void Dispose(bool disposing)
		{
			db.Dispose();
			base.Dispose(disposing);
		}
	}
}