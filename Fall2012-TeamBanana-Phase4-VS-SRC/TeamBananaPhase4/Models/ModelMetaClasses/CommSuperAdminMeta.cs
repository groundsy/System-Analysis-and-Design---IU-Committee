/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: CommSuperAdminMeta.cs
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

namespace TeamBananaPhase4.Models
{
	[MetadataType(typeof(CommSuperAdminMeta))]
    public partial class CommSuperAdmin
    {
	}

	public class CommSuperAdminMeta
	{
		[Required]
        [Display(Name="User Email")]
        public string SysUser_Email { get; set; }

		public int CommOwn_ID { get; set; }

		[Required]
        [Display(Name="Start Date")]
		public System.DateTime StartDate { get; set; }

        [Display(Name="End Date")]
        public Nullable<System.DateTime> EndDate { get; set; }

        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
    }
}
