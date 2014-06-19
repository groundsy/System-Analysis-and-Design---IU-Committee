/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: SysUserMeta.cs
 * Created by: Joel Haubold
 * Created date: 11-17-12
 * Primary Programmer: 
 * File description: Contains model validation meta data
 * 
 * Change Log:
 * Date programmer    change
 * 
 * 
*************************************************/
using System;
using System.Web.DynamicData;
using System.ComponentModel.DataAnnotations;
//------------------------------------------------------------------------------

namespace TeamBananaPhase4.Models
{
	[MetadataType(typeof(SysUserMeta))]
	public partial class SysUser
    {   
    }

	public class SysUserMeta
	{
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Phone { get; set; }
		public string OfficeInfo { get; set; }
		public string FacultyFlag { get; set; }
		public string FacultyRank { get; set; }
		public string StaffFlag { get; set; }
		public string StaffPosition { get; set; }
		public string StudentFlag { get; set; }
		public string IsITAdministrator { get; set; }
		public Nullable<int> Employer_ID { get; set; }
		public string CreatedBy { get; set; }
		public System.DateTime CreatedDate { get; set; }
	}
}
