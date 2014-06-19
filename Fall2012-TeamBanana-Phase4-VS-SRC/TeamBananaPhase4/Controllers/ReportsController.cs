/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: Controllers/ReportsController.cs
 * Created by: Joel Haubold
 * Created date: 11-23-12
 * Primary Programmer: Joel Haubold
 * File description: Controller for accessing reports
 * 
 * Change Log:
 * Date programmer    change
 * 
 * 
*************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamBananaPhase4.Models;

namespace TeamBananaPhase4.Controllers
{
	public class ReportsController : Controller
	{
		jashdownEntities db = new jashdownEntities();

		//
		// GET: /Reports/

		public ActionResult Index()
		{
			//populate dropdown list with possible reports
			//TODO: fix if statements
			List<SelectListItem> listOfReports = new List<SelectListItem>();

			//if (user is CSA) add csa reports
			if (db.CommSuperAdmin.Any(csa => csa.SysUser_Email == User.Identity.Name &&
											 csa.StartDate <= DateTime.Today &&
											 (csa.EndDate ?? DateTime.MaxValue) >= DateTime.Today))
			{
				listOfReports.Add(new SelectListItem
									{
										Text = "List of underfilled committees.",
										Value = "underfilled"
									});
				listOfReports.Add(new SelectListItem
									{
										Text = "List of committee's past and present chairs.",
										Value = "previousChairs"
									});
			}
			//if (user is CA) add CA reports
			if (db.CommMember.Any(cm => cm.Member_Email == User.Identity.Name &&
										cm.StartDate <=DateTime.Today &&
										cm.EndDate >= DateTime.Today &&
									   (cm.IsAdministrator == "Y" || cm.IsConvener == "Y")))
			{
				listOfReports.Add(new SelectListItem
									{
										Text = "List of discussion items what have no votes.",
										Value = "noVotes"
									});
			}
			//if(user is member) add member reports
			if (db.CommMember.Any(cm => cm.Member_Email == User.Identity.Name &&
										cm.StartDate <=DateTime.Today &&
										cm.EndDate >= DateTime.Today))
			{
				listOfReports.Add(new SelectListItem
									{
										Text = "List of discussion items that were approved.",
										Value = "approved"
									});
			}
			ViewBag.ListOfReports = listOfReports;
			return View();
		}
		[HttpPost]
		public ActionResult Index(string selectedReport)
		{
			//TODO: get current user
			string currentUser = User.Identity.Name;

			/********************************************************
			 * each report returns a query that contains the report information
			 * the query is nested group by so that there are multiple levels.
			 * -- Level 1
			 *   -- Level 2
			 *		   -- Data object 1
			 *		   -- Data object 2
			 *		   -- ...
			 *   -- Level 2
			 *		   -- Data object 1
			 *		   -- Data object 2
			 *		   -- ...
			 *	 ...
			 * --Level 1
			 *   ...
			 *******************************************************/
			switch (selectedReport)
			{
				case "underfilled":
					/************************************************************
					* REPORT: List of underfilled committees
					* **********************************************************/

					var underfilled = db.Comm.Where(c => c.IsArchived == "Y" &&
												  (c.MinMembers ?? 0) > c.CommMember.Count(cm => cm.StartDate < DateTime.Now && cm.EndDate >= DateTime.Today) &&
												  c.CommOwn.CommSuperAdmin.Any(csa => csa.SysUser_Email == currentUser &&
																					 csa.StartDate < DateTime.Now &&
																					 (csa.EndDate ?? DateTime.MaxValue) >= DateTime.Today))
									  .GroupBy(c => c.CommOwn);

					return View("underfilled", underfilled);

				case "previousChairs":
					/************************************************************
					* REPORT: List of previous chairs of committees
					* *********************************************************/

					var previousChairs = db.CommMember.Where(cm => ((cm.MemberRole_Role == "Chair" || cm.MemberRole_Role == "Co-chair")) &&
													   cm.Comm.IsArchived == "N" &&
													   cm.Comm.CommOwn.CommSuperAdmin.Any(csa => csa.SysUser_Email == currentUser &&
																						  csa.StartDate < DateTime.Now &&
																						  (csa.EndDate ?? DateTime.MaxValue) >= DateTime.Today))
										 .GroupBy(cm => new Comm.PrimaryKeyWithName { CommOwn_ID = cm.Comm.CommOwn_ID, ID = cm.Comm.ID, Name = cm.Comm.Name, commOwn = cm.CommOwn })
										 .GroupBy(c => c.Key.commOwn);

					return View("previousChairs", previousChairs);

				case "noVotes":
					/************************************************************
					* REPORT: List of discussion items with no votes
					* **********************************************************/
					//TODO: Test this and build view
					var noVotes = db.DiscItem.Where(di => (di.IsVoted == "Y" || di.IsAnonVoting == "Y") &&								//that are votable and
														   di.AnonVoting.Count() == 0 &&												//have no anonymous votes
														   di.Discussion.Count(d => d.Vote == "") == 0 &&								//and have no regular votes
														   di.Meeting.Comm.CommMember.Any(cm => cm.Member_Email == currentUser &&		//and belong to a committee that the currentuser
																								cm.StartDate <= DateTime.Today &&		//is a current CA of
																								cm.EndDate >= DateTime.Today &&
																							   (cm.IsAdministrator == "Y" || cm.IsConvener == "Y")) &&
														   di.Meeting.CreatedDate >= di.Meeting.Comm.CommMember.Where(cm => cm.Member_Email == currentUser &&  //and was created during the CA
																													cm.StartDate <= DateTime.Today &&  //current term.
																													cm.EndDate >= DateTime.Today).FirstOrDefault().StartDate &&
														   di.Meeting.CreatedDate <= di.Meeting.Comm.CommMember.Where(cm => cm.Member_Email == currentUser &&
																													cm.StartDate <= DateTime.Today &&
																													cm.EndDate >= DateTime.Today).FirstOrDefault().EndDate)
											 .GroupBy(di => new Meeting.PrimaryKey { Comm_CommOwn_ID = di.Meeting.Comm_CommOwn_ID, Comm_ID = di.Meeting_Comm_ID, DateTime = di.Meeting_DateTime })							//group the results by meeting
											 .GroupBy(m => new Comm.PrimaryKeyWithName { CommOwn_ID = m.Key.Comm_CommOwn_ID, ID = m.Key.Comm_ID}); //then group the results by committee
					return View("noVotes", noVotes);

				case "approved":
					/************************************************************
					* REPORT: List of discussion items with no votes
					* **********************************************************/
					//TODO: Test this and build view
					var approved = db.DiscItem.Where(di => di.Decision == "Approved" &&
														   di.Meeting.Comm.CommMember.Any(cm => cm.Member_Email == currentUser &&		//and belong to a committee that the currentuser
																								cm.StartDate <= DateTime.Today &&		//is a current member of
																								cm.EndDate >= DateTime.Today) &&
														   di.Meeting.CreatedDate >= di.Meeting.Comm.CommMember.Where(cm => cm.Member_Email == currentUser &&  //and was created during the members
																													cm.StartDate <= DateTime.Today &&  //current term.
																													cm.EndDate >= DateTime.Today).FirstOrDefault().StartDate &&
														   di.Meeting.CreatedDate <= di.Meeting.Comm.CommMember.Where(cm => cm.Member_Email == currentUser &&
																													cm.StartDate <= DateTime.Today &&
																													cm.EndDate >= DateTime.Today).FirstOrDefault().EndDate)
											 .GroupBy(di => new Meeting.PrimaryKey { Comm_CommOwn_ID = di.Meeting.Comm_CommOwn_ID, Comm_ID = di.Meeting_Comm_ID, DateTime = di.Meeting_DateTime})							//group the results by meeting
											 .GroupBy(m => new Comm.PrimaryKeyWithName { CommOwn_ID = m.Key.Comm_CommOwn_ID, ID = m.Key.Comm_ID}); //then group the results by committee

					return View("approvedDiscItems", approved); //the view for this is the same as the view for noVotes
			}

			//default
			//return invalid report
			ViewBag.Error = "Invalid report name";
			return View("Index");
		}

		protected override void Dispose(bool disposing)
		{
			db.Dispose();
			base.Dispose(disposing);
		}

	}
}
