/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: Controllers/SearchControllers.cs
 * Created by: Joel Haubold
 * Created date: 11-23-12
 * Primary Programmer: Joel Haubold
 * File description: Controller for search page
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
	public class SearchController : Controller
	{
		jashdownEntities db = new jashdownEntities();
		//
		// GET: /Search/


		/************************************************
		 * Function Name: Index()
		 * Input: None
		 * Output: Search view
		 * Description: Return empty search view.
		*************************************************/
		public ActionResult Index()
		{
			ViewBag.CommitteeDocs = null;
			ViewBag.DiscItemDocs = null;
			return View();
		}

		/************************************************
		 * Function Name: [HttpPost] Index
		 * Input: SearchString and Which committees selection from Search View
		 * Output: List of committee and discussion item documents matching search criteria
		 * Description: Performs search logic for documents.
		*************************************************/
		[HttpPost]//, ActionName("Search")]
		public ActionResult Index(string SearchString, string WhichCommittees)
		{
			if (SearchString == "")
			{
				ViewBag.Error = "You must enter a search string.";
				return View();
			}
			//get current user
			IQueryable<CommDocumentPKWithFilenameTags> CommitteeDocs = null;
			IQueryable<DiscItemDocumentWithoutImage> DiscItemDocs = null;
			
			switch (WhichCommittees)
			{
					//TODO: fix queries
				case "All":
					CommitteeDocs = db.CommDocument.Where(cd => cd.IsPublic == "Y" &&
																cd.Tags.Contains(SearchString)).Select(cd => new CommDocumentPKWithFilenameTags	{
																													Comm_CommOwn_ID = cd.Comm_CommOwn_ID,
																													Comm_ID = cd.Comm_ID,
																													Filename = cd.Filename,
																													Tags = cd.Tags,
																													Comm_Name = cd.Comm.Name,
																													Title = cd.Title });
					break;
				case "My":
					CommitteeDocs = db.CommDocument.Where(cd => cd.Comm.CommMember.Any(cm => cm.Member_Email == User.Identity.Name &&
																							 cm.StartDate <= DateTime.Today &&
																							 cm.EndDate >= DateTime.Today) &&
																cd.Tags.Contains(SearchString)).Select(cd => new CommDocumentPKWithFilenameTags { 
																													Comm_CommOwn_ID = cd.Comm_CommOwn_ID,
																													Comm_ID = cd.Comm_ID,
																													Filename = cd.Filename,
																													Tags = cd.Tags,
																													Comm_Name = cd.Comm.Name,
																													Title = cd.Title});

					DiscItemDocs = db.DiscItemDocument.Where(did => did.Tags.Contains(SearchString) &&
						did.DiscItem.Meeting.CreatedDate >= did.DiscItem.Meeting.Comm.CommMember.Where(cm => cm.Member_Email == User.Identity.Name && 
																											 cm.StartDate <= DateTime.Today &&
																											 cm.EndDate >= DateTime.Today).FirstOrDefault().StartDate &&
						did.DiscItem.Meeting.CreatedDate <= did.DiscItem.Meeting.Comm.CommMember.Where(cm => cm.Member_Email == User.Identity.Name &&
																											 cm.StartDate <= DateTime.Today &&
																											 cm.EndDate >= DateTime.Today).FirstOrDefault().EndDate)
																								.Select(cd => new DiscItemDocumentWithoutImage {
																															 DiscItem_Meeting_Comm_CommOwn_ID = cd.DiscItem_Meeting_Comm_CommOwn_ID,
																															 DiscItem_Meeting_Comm_ID = cd.DiscItem_Meeting_Comm_ID,
																															 DiscItem_Meeting_DateTime = cd.DiscItem_Meeting_DateTime,
																															 Filename = cd.Filename,
																															 Tags = cd.Tags});

					break;
			}
			if (CommitteeDocs.Count() == 0 && DiscItemDocs != null && DiscItemDocs.Count() == 0)
			{
				ViewBag.Error = "No search results";
			}
			ViewBag.DiscItemDocs = DiscItemDocs;
			ViewBag.CommitteeDocs = CommitteeDocs;
			return View();
		}

		protected override void Dispose(bool disposing)
		{
			db.Dispose();
			base.Dispose(disposing);
		}
	}
}
