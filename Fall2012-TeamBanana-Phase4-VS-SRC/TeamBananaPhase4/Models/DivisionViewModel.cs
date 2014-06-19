/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: models\DivisionViewModel.cs
 * Created by: Joel Haubold
 * Created date: 12-1-12
 * Primary Programmer: Joel Haubold 
 * File description: Contains a class DivisionViewModel that contains data displayed by the
 *					 division\details view. The division controller fill an object of this
 *					 class with data and passes it to the details view.
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

namespace TeamBananaPhase4.Models
{
	public class DivisionViewModel
	{
		public bool isCSA { get; set; }
		public string parentDivisionName { get; set; }
		public int parentDivisionID { get; set; }
		public string divisionName { get; set; }
		public int divisionID { get; set; }
		public string divisionType { get; set; }
		public string childDivisionType { get; set; }
		public List<GenericDivision> divisionList { get; set; }
		public List<Comm> committeeList { get; set; }
		public List<CommSuperAdmin> committeeSuperAdminList { get; set; }
	}
}