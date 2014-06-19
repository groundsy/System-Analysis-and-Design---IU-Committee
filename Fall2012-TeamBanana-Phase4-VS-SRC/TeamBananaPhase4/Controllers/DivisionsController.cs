/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: 
 * Created by: Joel Haubold
 * Created date: 11-30-12
 * Primary Programmer: Joel Haubold 
 * File description: Controller to display division hierarchy to the public user
 * 
 * Change Log:
 * Date programmer    change
 * 
 * 
*************************************************/


//TODO: make viewModel and stop using viewbag.
//Add comments
//TODO: check for CSA? and add create button for committees?
//TODO: prettify view
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamBananaPhase4.Models;

namespace TeamBananaPhase4.Controllers
{
    public class DivisionsController : Controller
    {
		private jashdownEntities db = new jashdownEntities();

		//
        // GET: /Report/
        public ActionResult Index(int primaryKey1 = 0)
        {
			//this controller receives one int which is the comm_own_id a division.
			//Hierarchy
			//University
			// +--Campus
			//       +--Unit
			//       +--School
			//             +--Unit

			//create object that will be passed to the view.
			DivisionViewModel divisionViewModel = new DivisionViewModel();

			//find division
			CommOwn commOwn = db.CommOwn.Find(primaryKey1);
			
			//if division doesn't exist (or id = 0) goto top level view
			if (commOwn == null)
			{
				divisionViewModel.parentDivisionID = 0;
				divisionViewModel.parentDivisionName = "";
				divisionViewModel.divisionName = "Welcome";
				divisionViewModel.divisionType = "";
				divisionViewModel.childDivisionType = "University";
				divisionViewModel.divisionList = db.University.Select(u => new GenericDivision { CommOwn_ID = u.CommOwn_ID, Name = u.Name }).ToList();
				divisionViewModel.committeeList = null;
				divisionViewModel.committeeSuperAdminList = null;
				return View("details", divisionViewModel);
			}
			
			//find division committees
			//get all non-archived committees if user is CSA
			if (db.CommSuperAdmin.Any(csa => csa.CommOwn_ID == commOwn.ID &&
											 csa.StartDate <= DateTime.Today &&
											(csa.EndDate ?? DateTime.MaxValue) >= DateTime.Today &&
											 csa.SysUser_Email == User.Identity.Name))
			{
				divisionViewModel.committeeList = db.Comm.Where(c => c.CommOwn_ID == commOwn.ID && c.IsArchived == "N").ToList();
			}
			else //or if user is not csa just publicly listed non-archived committees
			{
				divisionViewModel.committeeList = db.Comm.Where(c => c.CommOwn_ID == commOwn.ID && c.IsArchived == "N" && c.IsListedPublicly == "Y").ToList();
			}

			divisionViewModel.isCSA = db.CommSuperAdmin.Any(csa =>   csa.CommOwn_ID == commOwn.ID &&
																	 csa.StartDate <= DateTime.Today &&
																	(csa.EndDate ?? DateTime.MaxValue) >= DateTime.Today &&
											  						 csa.SysUser_Email == User.Identity.Name);

			//set current division id
			divisionViewModel.divisionID = commOwn.ID;
			
			//find all division CSA if the user is an IT Admin
			if (db.SysUser.Any(su => su.Email == User.Identity.Name &&
									 su.IsITAdministrator == "Y"))
			{
				divisionViewModel.committeeSuperAdminList = db.CommSuperAdmin.Where(csa => csa.CommOwn_ID == commOwn.ID).ToList();
			}
			else
			{
				divisionViewModel.committeeSuperAdminList = null;
			}

			//look for passed in comm own id in university, campus, school, and unit.
			//once found fill divisionViewModel with data about division

			//look in university
			University university;
			Campus campus;
			School school;
			Unit unit;
			
			if ((university = db.University.FirstOrDefault(u => u.CommOwn_ID == commOwn.ID)) != null)
			{
				divisionViewModel.parentDivisionName = "Welcome page";
				divisionViewModel.parentDivisionID = 0;
				divisionViewModel.divisionName = university.Name;
				divisionViewModel.divisionType = "University";
				divisionViewModel.childDivisionType = "Campuses";
				divisionViewModel.divisionList = university.Campus.Select(c => new GenericDivision { CommOwn_ID = c.CommOwn_ID, Name = c.Name }).ToList();
			}
			else if ((campus = db.Campus.FirstOrDefault(c => c.CommOwn_ID == commOwn.ID)) != null)
			{
				divisionViewModel.parentDivisionName = campus.University.Name;
				divisionViewModel.parentDivisionID = campus.University.CommOwn_ID;
				divisionViewModel.divisionName = campus.Name;
				divisionViewModel.divisionType = "Campus";
				divisionViewModel.childDivisionType = "Schools and Units";
				divisionViewModel.divisionList = campus.School.Select(c => new GenericDivision { CommOwn_ID = c.CommOwn_ID, Name = c.Name })
								  .Union(campus.Unit.Where(u => u.School_Campus_Code == null).Select(u => new GenericDivision { CommOwn_ID = u.CommOwn_ID, Name = u.Name })).ToList();
				//ViewBag.divisionList.addRange(campus.Unit.Select(u => new GenericDivision { CommOwn_ID = u.CommOwn_ID, Name = u.Name }).ToList());
			}
			else if ((school = db.School.FirstOrDefault(c => c.CommOwn_ID == commOwn.ID)) != null)
			{
				divisionViewModel.parentDivisionName = school.Campus.Name;
				divisionViewModel.parentDivisionID = school.Campus.CommOwn_ID;
				divisionViewModel.divisionName = school.Name;
				divisionViewModel.divisionType = "School";
				divisionViewModel.childDivisionType = "Units";
				divisionViewModel.divisionList = school.Unit.Select(u => new GenericDivision { CommOwn_ID = u.CommOwn_ID, Name = u.Name }).ToList();
			}
			else if ((unit = db.Unit.FirstOrDefault(c => c.CommOwn_ID == commOwn.ID)) != null)
			{
				divisionViewModel.parentDivisionName = (unit.School == null) ? unit.Campus.Name : unit.School.Name;
				divisionViewModel.parentDivisionID = (unit.School == null) ? unit.Campus.CommOwn_ID : unit.School.CommOwn_ID;
				divisionViewModel.divisionName = unit.Name;
				divisionViewModel.childDivisionType = "Campuses";
				divisionViewModel.divisionType = "";
				divisionViewModel.divisionList = null;
				//unit has no division children
			}
			else
			{
				//not found
			}


			//select view based
			
			//need to send to view:
			//division name
			//division's division children (GenericDivision)
			//division's committee children (Comm)
            return View("details", divisionViewModel);
        }

		protected override void Dispose(bool disposing)
		{
			db.Dispose();
			base.Dispose(disposing);
		}
    }
}
