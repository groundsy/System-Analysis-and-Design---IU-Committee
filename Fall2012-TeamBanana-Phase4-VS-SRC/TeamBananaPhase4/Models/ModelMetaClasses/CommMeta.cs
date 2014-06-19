/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: CommMeta.cs
 * Created by: Joel Haubold
 * Created date: 11-17-12
 * Primary Programmer: 
 * File description: Contains model validation meta data
 * 
 * Change Log:
 *  Date     Programmer     Change
 * 11/28/12     dt          Adding require to required member in the database
 * 
*************************************************/

using System;
using System.Web.DynamicData;
using System.ComponentModel.DataAnnotations;

namespace TeamBananaPhase4.Models

{
	
	[MetadataType(typeof(CommMeta))]
	public partial class Comm
    {
		public class PrimaryKeyWithName
		{
			public int CommOwn_ID { get; set; }
			public int ID { get; set; }
			public string Name { get; set; }
			public CommOwn commOwn { get;set;}
		}
	}

	public class CommMeta
	{
        [Required]
		public int CommOwn_ID { get; set; }

        [Required]
        public int ID { get; set; }

        [Required]
		[StringLength(30, ErrorMessage = "Committee name must be 30 characters or less.")]
		public string Name { get; set; }

        [Required]
		[Display (Name="Effective Date")]
		
        public System.DateTime EffectiveDate { get; set; }
		
		[Range(0,10000,ErrorMessage="The minimum must not be less than 0")]
		[Display(Name = "Minimum number of members")]
        public Nullable<int> MinMembers { get; set; }

		[Range(0, 10000, ErrorMessage = "The maximum must not be less than 0")]
		[Display(Name = "Maximum number of members")]
        public Nullable<int> MaxMembers { get; set; }
        
		[Display (Name="Member term length")]
		public Nullable<int> MembershipYears { get; set; }

        [Required]
        public string Type { get; set; }

        [Required(ErrorMessage = "Please Enter Y or N")]
        [Display(Name = "Listed publicly?")]
        [EnumDataType(typeof(TeamBananaPhase4.Controllers.enumYN), ErrorMessage = "Please Enter Y or N")]
        public string IsListedPublicly { get; set; }

        
        public string CreatedBy { get; set; }
		public System.DateTime CreatedDate { get; set; }

		[Display(Name = "Creation Comments")]
        public string CreationComments { get; set; }
        public string IsArchived { get; set; }
        public string ArchivedBy { get; set; }
        public Nullable<System.DateTime> ArchivedDate { get; set; }
        public string ArchivedComments { get; set; }
    }
}
